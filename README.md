# MagSec Encrypter

A Windows desktop app for encrypting **text** and **image files** with authenticated, password-based encryption — wrapped in a polished, space-themed UI.

![Platform](https://img.shields.io/badge/platform-Windows-0a7bdc)
![.NET](https://img.shields.io/badge/.NET-10.0--windows-512bd4)
![UI](https://img.shields.io/badge/UI-WinForms-2d4dde)

## Features

- **Text encryption** — encrypt/decrypt any text and copy the Base64 result to the clipboard.
- **Image encryption** — encrypt image files (`.png`, `.jpg`, `.jpeg`, `.bmp`, `.gif`, `.webp`) into a `.mseimg` container and restore them later, with an in-app preview.
- **Up to four access keys** — combine 1–4 keys into a single secret; the same combination is required to decrypt.
- **Key tools** — generate a strong random key, toggle key visibility, and watch a live key-strength indicator.
- **Drag & drop** — drop an image or a `.mseimg` file straight onto the window.

## Security

MagSec Encrypter uses modern, authenticated encryption from the .NET cryptography stack:

| Aspect | Detail |
| --- | --- |
| Cipher | **AES-256-GCM** (authenticated encryption) |
| Key derivation | **PBKDF2-HMAC-SHA512**, 600,000 iterations |
| Salt | 16 bytes, random per operation |
| Nonce | 12 bytes, random per operation |
| Auth tag | 16 bytes |
| Header integrity | The full header (format prefix, version, KDF parameters, salt, nonce) is bound into the GCM tag as **associated data**, so any tampering is detected on decrypt. |
| Memory hygiene | Derived keys and secret bytes are zeroed (`CryptographicOperations.ZeroMemory`) after use. |
| Key policy | A minimum combined key length is enforced to discourage weak secrets. |

### Payload format

Encrypted output is a self-describing **v2** binary payload (Base64-encoded for text):

```
prefix(4) | version(1) | kdfId(1) | iterations(4, big-endian) | salt(16) | nonce(12) | tag(16) | ciphertext
```

Storing the KDF parameters in the header provides **crypto agility** — the work factor or hash can change in future versions without breaking existing files. Legacy **v1** payloads (`TXT1` / `IMG1`) remain decryptable for backwards compatibility.

> ⚠️ **There is no key recovery.** If you lose your key combination, the data cannot be decrypted. Store your keys somewhere safe.

## Requirements

- Windows
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (target framework `net10.0-windows`)

## Build & run

```bash
dotnet build
dotnet run
```

Or open the project in Visual Studio and press **F5**.

## Usage

1. Enter one or more **access keys** in the left rail (or click **Generate** for a strong random key).
2. **Text:** type or paste into the Input box, then **Encrypt** / **Decrypt**. Use **Copy output** to grab the result.
3. **Images:** **Open image** → **Encrypt image** to save a `.mseimg` file. To restore, **Open encrypted file** → **Decrypt image**.
4. Use the **same key combination** for decryption that you used to encrypt.

## Project layout

| File | Purpose |
| --- | --- |
| `Form1.cs` | App logic, cryptography, custom controls, and the animated background |
| `Form1.Designer.cs` | WinForms layout |
| `Program.cs` | Entry point |
| `TextEncryptorApp.csproj` | Project configuration |

## License

No license has been specified for this project.
