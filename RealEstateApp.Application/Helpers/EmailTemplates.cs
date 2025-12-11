namespace RealEstateApp.Application.Helpers
{
    public static class EmailTemplates
    {
        public static (string HtmlBody, string TextBody) BuildAccountConfirmationEmail(
            string firstName,
            string verificationUri,
            string origin)
        {
            var displayName = string.IsNullOrWhiteSpace(firstName) ? "usuario" : firstName;
            var safeOrigin = string.IsNullOrWhiteSpace(origin) ? "" : origin.TrimEnd('/');
            var logoUrl = string.IsNullOrWhiteSpace(safeOrigin)
                ? "/Images/logo.png"
                : $"{safeOrigin}/Images/logo.png";

            var html = $@"
<!doctype html>
<html lang=""es"">
<head>
  <meta charset=""utf-8""/>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
  <title>Confirma tu cuenta - RealEstateApp</title>
</head>
<body style=""margin:0;padding:0;background:#f6f7fb;font-family:Inter, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
    <tr>
      <td align=""center"" style=""padding:28px 16px;"">
        <table width=""600"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 8px 30px rgba(2,6,23,0.08);"">
          <tr>
            <td style=""padding:20px;text-align:center;background:#0a0a0a;"">
              <img src=""{logoUrl}"" alt=""RealEstateApp"" width=""84"" style=""display:block;margin:0 auto 8px;object-fit:contain;"" />
              <h1 style=""margin:0;color:#ffffff;font-size:20px;letter-spacing:0.2px;"">RealEstateApp</h1>
            </td>
          </tr>
          <tr>
            <td style=""padding:28px 32px 24px;color:#111827;"">
              <p style=""margin:0 0 12px;font-size:16px;""><strong>Hola {displayName},</strong></p>
              <p style=""margin:0 0 18px;color:#6b7280;font-size:14px;line-height:1.5;"">
                ¡Gracias por registrarte en RealEstateApp! Antes de empezar, necesitamos que confirmes tu correo electrónico.
              </p>

              <div style=""text-align:center;margin:22px 0;"">
                <a href=""{verificationUri}"" target=""_blank"" rel=""noopener noreferrer"" style=""background:linear-gradient(135deg,#c9a227,#d4b445);color:#0a0a0a;text-decoration:none;padding:12px 22px;border-radius:10px;font-weight:700;display:inline-block;font-size:15px;"">
                  Confirmar mi correo
                </a>
              </div>

              <p style=""margin:0 0 12px;color:#6b7280;font-size:13px;"">
                Si el botón no funciona, copia y pega esta URL en tu navegador:
              </p>
              <p style=""word-break:break-all;font-size:12px;color:#0b1220;margin:0 0 18px;"">{verificationUri}</p>

              <p style=""margin:0 0 8px;color:#6b7280;font-size:13px;"">
                Este enlace expirará en 24 horas. Si no solicitaste este registro, ignora este correo o contáctanos.
              </p>

              <hr style=""border:none;border-top:1px solid #eef2f6;margin:20px 0;"" />

              <p style=""margin:0;color:#9ca3af;font-size:12px;"">
                ¿Necesitas ayuda? Escríbenos a <a href=""mailto:realstateapp@gmail.com"" style=""color:#c9a227;text-decoration:none;"">realstateapp@gmail.com</a>
              </p>

              <p style=""margin:16px 0 0;color:#9ca3af;font-size:11px;"">
                RealEstateApp • Premium Properties
              </p>
            </td>
          </tr>

          <tr>
            <td style=""background:#fafafa;padding:12px 20px;text-align:center;font-size:11px;color:#9aa0a6;"">
              Si no creaste esta cuenta, simplemente ignora este correo.
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";

            var text = $@"
Hola {displayName},

Gracias por registrarte en RealEstateApp.

Confirma tu correo visitando esta URL:
{verificationUri}

Este enlace expirará en 24 horas.

Si no solicitaste este registro, ignora este correo.

Soporte: realstateapp@gmail.com
";

            return (html, text);
        }

        public static (string HtmlBody, string TextBody) BuildResetPasswordEmail(
            string firstName,
            string resetUri,
            string origin,
            bool isApiMode,
            string? resetToken = null,
            int expiryHours = 24)
        {
            var displayName = string.IsNullOrWhiteSpace(firstName) ? "usuario" : firstName;
            var safeOrigin = string.IsNullOrWhiteSpace(origin) ? "" : origin.TrimEnd('/');
            var logoUrl = string.IsNullOrWhiteSpace(safeOrigin)
                ? "/Images/logo.png"
                : $"{safeOrigin}/Images/logo.png";

            if (!isApiMode)
            {
                var html = $@"
<!doctype html>
<html lang=""es"">
<head>
  <meta charset=""utf-8""/>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0""/>
  <title>Restablecer contraseña - RealEstateApp</title>
</head>
<body style=""margin:0;padding:0;background:#f6f7fb;font-family:Inter, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
    <tr>
      <td align=""center"" style=""padding:28px 16px;"">
        <table width=""600"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 8px 30px rgba(2,6,23,0.08);"">
          <tr>
            <td style=""padding:20px;text-align:center;background:#0a0a0a;"">
              <img src=""{logoUrl}"" alt=""RealEstateApp"" width=""84"" style=""display:block;margin:0 auto 8px;object-fit:contain;"" />
              <h1 style=""margin:0;color:#ffffff;font-size:20px;letter-spacing:0.2px;"">RealEstateApp</h1>
            </td>
          </tr>
          <tr>
            <td style=""padding:28px 32px 24px;color:#111827;"">
              <p style=""margin:0 0 12px;font-size:16px;""><strong>Hola {displayName},</strong></p>
              <p style=""margin:0 0 18px;color:#6b7280;font-size:14px;line-height:1.5;"">
                Recibimos una solicitud para restablecer la contraseña de tu cuenta. Haz clic en el botón para continuar.
              </p>

              <div style=""text-align:center;margin:22px 0;"">
                <a href=""{resetUri}"" target=""_blank"" rel=""noopener noreferrer"" style=""background:linear-gradient(135deg,#c9a227,#d4b445);color:#0a0a0a;text-decoration:none;padding:12px 22px;border-radius:10px;font-weight:700;display:inline-block;font-size:15px;"">
                  Restablecer mi contraseña
                </a>
              </div>

              <p style=""margin:0 0 12px;color:#6b7280;font-size:13px;"">
                Si el botón no funciona, copia y pega esta URL en tu navegador:
              </p>
              <p style=""word-break:break-all;font-size:12px;color:#0b1220;margin:0 0 18px;"">{resetUri}</p>

              <p style=""margin:0 0 8px;color:#6b7280;font-size:13px;"">
                El enlace expirará en {expiryHours} horas. Si no solicitaste este restablecimiento, ignora este correo o contacta soporte.
              </p>

              <hr style=""border:none;border-top:1px solid #eef2f6;margin:20px 0;"" />

              <p style=""margin:0;color:#9ca3af;font-size:12px;"">
                ¿Necesitas ayuda? Escríbenos a <a href=""mailto:realstateapp@gmail.com"" style=""color:#c9a227;text-decoration:none;"">realstateapp@gmail.com</a>
              </p>

              <p style=""margin:16px 0 0;color:#9ca3af;font-size:11px;"">
                RealEstateApp • Premium Properties
              </p>
            </td>
          </tr>

          <tr>
            <td style=""background:#fafafa;padding:12px 20px;text-align:center;font-size:11px;color:#9aa0a6;"">
              Si no solicitaste este restablecimiento, simplemente ignora este correo.
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";

                var text = $@"
Hola {displayName},

Recibimos una solicitud para restablecer la contraseña de tu cuenta.

Utiliza la siguiente URL para restablecerla (expira en {expiryHours} horas):
{resetUri}

Si no solicitaste este restablecimiento, ignora este correo.

Soporte: realstateapp@gmail.com
";

                return (html, text);
            }
            else
            {
                var htmlApi = $@"
<!doctype html>
<html lang=""es"">
<head><meta charset=""utf-8""/></head>
<body style=""font-family:Inter,Arial,sans-serif;color:#111827;"">
  <p>Hola {displayName},</p>
  <p>Solicitaste restablecer tu contraseña. Usa este token en la aplicación para completar el proceso:</p>
  <pre style=""background:#f6f7fb;padding:12px;border-radius:6px;"">{resetToken}</pre>
  <p>El token expira en {expiryHours} horas.</p>
  <p>Si no solicitaste este token, ignora este mensaje.</p>
  <p>Soporte: realstateapp@gmail.com</p>
</body>
</html>
";

                var textApi = $@"
Hola {displayName},

Solicitaste restablecer tu contraseña. Usa este token en la aplicación para completar el proceso:

{resetToken}

El token expira en {expiryHours} horas.

Si no solicitaste este token, ignora este mensaje.

Soporte: realstateapp@gmail.com
";

                return (htmlApi, textApi);
            }
        }
    }
}
