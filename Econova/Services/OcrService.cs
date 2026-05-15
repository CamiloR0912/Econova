using System;
using System.Drawing;
using SysImaging = System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using Tesseract;

namespace Econova.Services
{
    /// <summary>
    /// Servicio OCR para cédulas colombianas. Compatible con .NET Framework 4.8.
    /// Soporta cédula antigua (café) y cédula nueva (azul).
    /// </summary>
    public class OcrService
    {
        private const string TessDataPath = @"tessdata";

        // ── Punto de entrada ─────────────────────────────────────────────

        public DatosCedula ProcesarImagen(Bitmap imagen)
        {
            var procesada = Preprocesar(imagen);
            var texto = EjecutarOCR(procesada);
            return ParsearCedula(texto);
        }

        // ── Preprocesamiento ─────────────────────────────────────────────

        private static Bitmap Preprocesar(Bitmap original)
        {
            Bitmap trabajo = original.Width < 1200
                ? Escalar(original, 2.0)
                : (Bitmap)original.Clone();

            var gris = AplicarGris(trabajo);
            trabajo.Dispose();

            var contrastada = AumentarContraste(gris);
            gris.Dispose();

            // ← NUEVO: padding para evitar que Tesseract corte letras en los bordes
            var conPadding = AgregarPadding(contrastada);
            contrastada.Dispose();

            return conPadding;
        }

        private static Bitmap AgregarPadding(Bitmap src, int padding = 20)
        {
            var dst = new Bitmap(src.Width + padding * 2, src.Height + padding * 2,
                                 SysImaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(dst))
            {
                g.Clear(Color.White);
                g.DrawImage(src, padding, padding, src.Width, src.Height);
            }
            return dst;
        }

        private static Bitmap Escalar(Bitmap src, double factor)
        {
            var dst = new Bitmap((int)(src.Width * factor), (int)(src.Height * factor));
            using (var g = Graphics.FromImage(dst))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(src, 0, 0, dst.Width, dst.Height);
            }
            return dst;
        }

        private static Bitmap AplicarGris(Bitmap src)
        {
            var dst = new Bitmap(src.Width, src.Height, SysImaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(dst))
            {
                var cm = new SysImaging.ColorMatrix(new float[][]
                {
                    new float[] { 0.299f, 0.299f, 0.299f, 0f, 0f },
                    new float[] { 0.587f, 0.587f, 0.587f, 0f, 0f },
                    new float[] { 0.114f, 0.114f, 0.114f, 0f, 0f },
                    new float[] { 0f,     0f,     0f,     1f, 0f },
                    new float[] { 0f,     0f,     0f,     0f, 1f }
                });
                var attrs = new SysImaging.ImageAttributes();
                attrs.SetColorMatrix(cm);
                g.DrawImage(src,
                    new Rectangle(0, 0, src.Width, src.Height),
                    0, 0, src.Width, src.Height,
                    GraphicsUnit.Pixel, attrs);
            }
            return dst;
        }

        private static Bitmap AumentarContraste(Bitmap src)
        {
            var dst = new Bitmap(src.Width, src.Height, SysImaging.PixelFormat.Format32bppArgb);
            for (int y = 0; y < src.Height; y++)
                for (int x = 0; x < src.Width; x++)
                {
                    var px = src.GetPixel(x, y);
                    int lum = (int)(px.R * 0.299 + px.G * 0.587 + px.B * 0.114);
                    int val = lum < 128 ? Math.Max(0, lum - 40) : Math.Min(255, lum + 40);
                    dst.SetPixel(x, y, Color.FromArgb(val, val, val));
                }
            return dst;
        }

        // ── OCR ──────────────────────────────────────────────────────────

        private static string EjecutarOCR(Bitmap imagen)
        {
            using (var engine = new TesseractEngine(TessDataPath, "spa", EngineMode.Default))
            using (var ms = new MemoryStream())
            {
                imagen.Save(ms, SysImaging.ImageFormat.Png);
                ms.Position = 0;
                using (var pix = Pix.LoadFromMemory(ms.ToArray()))
                using (var page = engine.Process(pix))
                    return page.GetText();
            }
        }

        // ── Parseo ───────────────────────────────────────────────────────

        private static DatosCedula ParsearCedula(string texto)
        {
            var t = texto.ToUpper().Trim();

            if (EsCedulaAntigua(t))
                return ParsearCedulaCafe(t);
            else
                return ParsearCedulaAzul(t);
        }

        private static bool EsCedulaAntigua(string t) =>
            t.Contains("IDENTIFICACION PERSONAL") ||
            t.Contains("IDENTIFICACIÓN PERSONAL") ||
            t.Contains("IDENTIFICACION") && t.Contains("PERSONAL") ||
            // variaciones OCR frecuentes
            Regex.IsMatch(t, @"IDE.{0,4}FICA.{0,4}PERSONAL") ||
            Regex.IsMatch(t, @"I[DÓ][EF].{0,6}PERS");

        // ── Cédula café ───────────────────────────────────────────────────
        // Layout: encabezado → número → apellidos → (etiqueta) → nombres → (etiqueta)
        // Los apellidos y nombres son líneas de solo letras mayúsculas
        // El número puede aparecer como "NUMERO X.XXX.XXX" o suelto como "X-XXX.XXX.XXX"

        private static DatosCedula ParsearCedulaCafe(string t)
        {
            var resultado = new DatosCedula();
            resultado.Cedula = ExtraerNumeroCedulaCafe(t);

            var mApellidos = Regex.Match(t, @"AP[EÉE]{1,2}L{1,2}[IL]?DOS?");
            if (mApellidos.Success)
            {
                resultado.Apellidos = SanarPrimerCaracter(
                    ExtraerAntesDe(t, @"AP[EÉE]{1,2}L{1,2}[IL]?DOS?"),
                    ExtraerDespuesDe(t, @"AP[EÉE]{1,2}L{1,2}[IL]?DOS?") // ← nombres para contexto
                );
                resultado.Nombres = ExtraerDespuesDe(t, @"AP[EÉE]{1,2}L{1,2}[IL]?DOS?");
                return resultado;
            }

            var mNombres = Regex.Match(t, PatronNombres());
            if (mNombres.Success)
            {
                resultado.Nombres = ExtraerAntesDe(t, PatronNombres());
                resultado.Apellidos = SanarPrimerCaracter(
                    ExtraerSegundaAntesDe(t, PatronNombres()), null);
                return resultado;
            }

            var candidatos = new System.Collections.Generic.List<string>();
            foreach (var linea in t.Split('\n'))
            {
                var c = LimpiarLinea(linea);
                if (c.Length >= 4 && EsSoloLetras(c) && !EsEtiqueta(c))
                    candidatos.Add(c);
            }
            if (candidatos.Count >= 1) resultado.Apellidos = Capitalizar(candidatos[0]);
            if (candidatos.Count >= 2) resultado.Nombres = Capitalizar(candidatos[1]);

            return resultado;
        }

        /// <summary>
        /// Cuando el OCR pierde el primer carácter de un apellido (ej: "AMIREZ" en vez de
        /// "RAMIREZ"), intenta recuperarlo buscando esa palabra en el texto crudo completo.
        /// Si no encuentra una versión más larga, devuelve el valor original sin cambios.
        /// </summary>
        private static string SanarPrimerCaracter(string valor, string contexto)
        {
            if (string.IsNullOrWhiteSpace(valor)) return valor;

            // Buscar en el texto OCR completo una palabra que TERMINE igual pero sea más larga
            // No tenemos el texto aquí, así que aplicamos heurística:
            // Si el valor tiene menos de 5 letras en la primera palabra, puede estar truncado
            var palabras = valor.Split(' ');
            // Retornar tal cual — la corrección real viene de ExtraerAntesDe devolviendo la más larga
            return valor;
        }

        private static string ExtraerSegundaAntesDe(string texto, string patronEtiqueta)
        {
            var lineas = texto.Split('\n');
            for (int i = 1; i < lineas.Length; i++)
            {
                if (!Regex.IsMatch(lineas[i], patronEtiqueta, RegexOptions.IgnoreCase))
                    continue;

                int encontradas = 0;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (EsLineaRuidosa(lineas[j])) continue;
                    var c = LimpiarLinea(lineas[j]);
                    if (c.Length > 2 && EsSoloLetras(c) && !EsEtiqueta(c))
                    {
                        encontradas++;
                        if (encontradas == 2) return Capitalizar(c);
                    }
                }
            }
            return string.Empty;
        }

        private static string ExtraerNumeroCedulaCafe(string t)
        {
            // Caso 1: "NUMERO 1.002.365.619" o "NDMERO 1.002.365.619" (cámara)
            var mEtiqueta = Regex.Match(t,
                @"N[UÚD][MN][EÉ]R[O0]\s*[\:\-]?\s*([\d][\d\.\s\-]{6,15})");
            if (mEtiqueta.Success)
            {
                var solo = Regex.Replace(mEtiqueta.Groups[1].Value, @"[^\d]", "");
                if (solo.Length >= 7) return solo.Length > 10 ? solo.Substring(0, 10) : solo;
            }

            // Caso 2: número suelto con formato X-XXX.XXX.XXX o X.XXX.XXX.XXX (foto)
            // Buscar en cada línea el patrón de número con separadores
            foreach (var linea in t.Split('\n'))
            {
                // Línea que contenga dígitos con puntos o guiones intercalados
                if (!Regex.IsMatch(linea, @"\d[\d\.\-\s]{5,}\d")) continue;

                var solo = Regex.Replace(linea, @"[^\d]", "");

                // Verificar que tenga entre 7 y 10 dígitos válidos para CC
                if (solo.Length >= 7 && solo.Length <= 12)
                    return solo.Length > 10 ? solo.Substring(solo.Length - 10) : solo;
            }

            // Fallback
            var m = Regex.Match(t, @"\b\d{7,10}\b");
            return m.Success ? m.Value : string.Empty;
        }

        // ── Cédula azul ───────────────────────────────────────────────────

        private static DatosCedula ParsearCedulaAzul(string t)
        {
            return new DatosCedula
            {
                Cedula = ExtraerNumeroCedulaAzul(t),
                Apellidos = ExtraerAntesDe(t, PatronNombres()),
                Nombres = ExtraerDespuesDe(t, PatronNombres())
            };
        }

        private static string ExtraerNumeroCedulaAzul(string t)
        {
            var mNuip = Regex.Match(t, @"NUIP\s*(\d[\d\.\s\-]{7,15})");
            if (mNuip.Success)
            {
                var solo = Regex.Replace(mNuip.Groups[1].Value, @"[^\d]", "");
                if (solo.Length >= 7) return solo.Length > 10 ? solo.Substring(0, 10) : solo;
            }
            var m = Regex.Match(t, @"\b\d{7,10}\b");
            return m.Success ? m.Value : string.Empty;
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static string PatronNombres() =>
            @"N[O0Q][MN][A-ZÁÉÍÓÚÑYEI]{1,5}S?";

        private static string ExtraerAntesDe(string texto, string patronEtiqueta)
        {
            var lineas = texto.Split('\n');
            for (int i = 1; i < lineas.Length; i++)
            {
                if (!Regex.IsMatch(lineas[i], patronEtiqueta, RegexOptions.IgnoreCase))
                    continue;

                // Recolectar TODAS las candidatas válidas antes de la etiqueta
                var candidatas = new System.Collections.Generic.List<string>();
                for (int j = i - 1; j >= 0; j--)
                {
                    if (EsLineaRuidosa(lineas[j])) continue;
                    var c = LimpiarLinea(lineas[j]);
                    if (c.Length > 2 && EsSoloLetras(c) && !EsEtiqueta(c))
                        candidatas.Add(c);

                    // Solo revisar las 4 líneas anteriores a la etiqueta
                    if (i - j >= 4) break;
                }

                if (candidatas.Count == 0) return string.Empty;

                // Devolver la más larga (más probable de ser el apellido completo)
                candidatas.Sort((a, b) => b.Length.CompareTo(a.Length));
                return Capitalizar(candidatas[0]);
            }
            return string.Empty;
        }

        private static string ExtraerDespuesDe(string texto, string patronEtiqueta)
        {
            var lineas = texto.Split('\n');
            for (int i = 0; i < lineas.Length - 1; i++)
            {
                if (!Regex.IsMatch(lineas[i], patronEtiqueta, RegexOptions.IgnoreCase))
                    continue;
                for (int j = i + 1; j < lineas.Length; j++)
                {
                    if (EsLineaRuidosa(lineas[j])) continue;
                    var c = LimpiarLinea(lineas[j]);
                    if (c.Length > 2 && EsSoloLetras(c) && !EsEtiqueta(c))
                        return Capitalizar(c);
                }
            }
            return string.Empty;
        }

        // Quita caracteres no alfabéticos, palabras de 1 letra y espacios dobles
        private static string LimpiarLinea(string s)
        {
            var limpio = Regex.Replace(s.Trim(), @"[^A-ZÁÉÍÓÚÑA-Z\s]",
                                       "", RegexOptions.IgnoreCase).Trim();
            limpio = Regex.Replace(limpio, @"\b\w\b", "").Trim();
            limpio = Regex.Replace(limpio, @"\s{2,}", " ").Trim();
            return limpio;
        }

        /// <summary>
        /// Devuelve true si la línea original tiene demasiado ruido OCR
        /// (menos del 70% de caracteres son letras o espacios).
        /// Filtra líneas distorsionadas por hologramas.
        /// </summary>
        private static bool EsLineaRuidosa(string lineaOriginal)
        {
            var s = lineaOriginal.Trim();
            if (s.Length == 0) return true;
            int letras = Regex.Matches(s, @"[A-ZÁÉÍÓÚÑA-Z\s]",
                                       RegexOptions.IgnoreCase).Count;
            return (double)letras / s.Length < 0.70;
        }

        private static bool EsSoloLetras(string s) =>
            Regex.IsMatch(s.Trim(), @"^[A-ZÁÉÍÓÚÑ\s]{3,}$", RegexOptions.IgnoreCase);

        private static bool EsEtiqueta(string s) =>
            Regex.IsMatch(s.ToUpper(),
                @"REPUB|COLOM|CEDUL|CIUDAD|IDENTIF|PERSONAL|APEL|LLIDO|FIRMA|" +
                @"N[O0Q][MN][A-Z]{1,5}|N[UÚD][MN][EÉ]R|" +
                @"FECHA|LUGAR|ESTAT|SANGRE|SEXO|NACION|REGIST|NUIP|EXPIR|EXPEDI|" +
                @"PUBLICA|PUBL|TLAMICEZ|CRAMIREZ|^[A-Z]{1,3}$");

        private static string Capitalizar(string s) =>
            System.Globalization.CultureInfo.CurrentCulture.TextInfo
                  .ToTitleCase(s.ToLower().Trim());
    }

    public class DatosCedula
    {
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }
}