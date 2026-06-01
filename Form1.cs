using System.Buffers.Binary;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace TextEncryptorApp;

public partial class Form1 : Form
{
    private const int SaltSize = 16;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int KeySize = 32;
    private const int Pbkdf2Iterations = 600_000;
    private const int LegacyPbkdf2Iterations = 200_000;
    private const int MinimumKeyLength = 8;
    private const byte FormatVersion = 2;
    private const byte KdfSha256 = 1;
    private const byte KdfSha512 = 2;

    // V2 self-describing format. V1 prefixes are still decryptable for backwards compatibility.
    private const string TextPrefix = "TXT2";
    private const string ImagePrefix = "IMG2";
    private const string LegacyTextPrefix = "TXT1";
    private const string LegacyImagePrefix = "IMG1";

    // prefix(4) + version(1) + kdfId(1) + iterations(4) + salt(16) + nonce(12)
    private const int V2HeaderLength = 4 + 1 + 1 + 4 + SaltSize + NonceSize;
    private const int LegacyHeaderLength = 4 + SaltSize + NonceSize + TagSize;

    private static readonly Color VoidColor = Color.FromArgb(3, 0, 20);
    private static readonly Color PanelColor = Color.FromArgb(12, 17, 39);
    private static readonly Color TextMainColor = Color.FromArgb(245, 248, 255);
    private static readonly Color TextMutedColor = Color.FromArgb(148, 163, 184);
    private static readonly Color CyanColor = Color.FromArgb(14, 165, 233);
    private static readonly Color TealColor = Color.FromArgb(45, 212, 191);
    private static readonly Color IndigoColor = Color.FromArgb(99, 102, 241);
    private static readonly Color EmeraldColor = Color.FromArgb(16, 185, 129);
    private static readonly Color DangerColor = Color.FromArgb(244, 63, 94);
    private readonly List<Star> stars = [];
    private readonly List<ShootingStar> shootingStars = [];
    private readonly System.Windows.Forms.Timer starfieldTimer = new() { Interval = 33 };
    private readonly Random random = new();
    private float nebulaPhase;
    private string? selectedImagePath;
    private string? selectedEncryptedImagePath;
    private bool keysVisible;

    public Form1()
    {
        InitializeComponent();
        ApplyTheme();
        LoadBrandImage();
        ConfigureDragAndDrop();
        ConfigureKeyFeedback();
        InitializeStarfield();
    }

    private void ConfigureKeyFeedback()
    {
        foreach (TextBox keyBox in new[] { key1TextBox, key2TextBox, key3TextBox, key4TextBox })
        {
            keyBox.TextChanged += (_, _) => UpdateKeyStrength();
        }

        UpdateKeyStrength();
    }

    private void ApplyTheme()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        DoubleBuffered = true;

        heroPanel.AccentColor = IndigoColor;
        keyRailPanel.AccentColor = CyanColor;
        textPanel.AccentColor = TealColor;
        imagePanel.AccentColor = IndigoColor;
        previewPanel.AccentColor = IndigoColor;

        ConfigureButton(encryptTextButton, CyanColor);
        ConfigureButton(decryptTextButton, TealColor);
        ConfigureButton(copyTextButton, IndigoColor);
        ConfigureButton(clearAllButton, Color.FromArgb(30, 35, 62), isSecondary: true);
        ConfigureButton(selectImageButton, CyanColor);
        ConfigureButton(encryptImageButton, TealColor);
        ConfigureButton(selectEncryptedImageButton, IndigoColor);
        ConfigureButton(decryptImageButton, CyanColor);
        ConfigureButton(generateKeyButton, EmeraldColor);
        ConfigureButton(toggleKeysButton, Color.FromArgb(36, 45, 72), isSecondary: true);
        toggleKeysButton.GlowColor = CyanColor;

        StyleTextBox(key1TextBox);
        StyleTextBox(key2TextBox);
        StyleTextBox(key3TextBox);
        StyleTextBox(key4TextBox);
        StyleTextBox(inputTextBox);
        StyleTextBox(outputTextBox);
        inputTextBox.ApplyTheme(PanelColor, TextMainColor, Color.FromArgb(18, 28, 52), Color.FromArgb(95, 108, 255));
        outputTextBox.ApplyTheme(PanelColor, TextMainColor, Color.FromArgb(18, 28, 52), Color.FromArgb(65, 200, 190));

        brandPictureBox.BackColor = Color.Transparent;
        imagePreviewBox.BackColor = Color.Transparent;
        imagePathValueLabel.ForeColor = TextMutedColor;
        encryptedImagePathValueLabel.ForeColor = TextMutedColor;
        sideInfoLabel.ForeColor = TextMutedColor;
        heroMetaLabel.ForeColor = TextMutedColor;
        imageDropHintLabel.ForeColor = TextMutedColor;

        titleLabel.Font = new Font("Century Gothic", 25F, FontStyle.Bold);
        heroKickerLabel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        subtitleLabel.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
        railTitleLabel.Font = new Font("Century Gothic", 16.5F, FontStyle.Bold);
        textPanelDescription.Font = new Font("Century Gothic", 16.5F, FontStyle.Bold);
        imagePanelTitle.Font = new Font("Century Gothic", 16.5F, FontStyle.Bold);
        inputLabel.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
        outputLabel.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);

        clearAllButton.FillColor = Color.FromArgb(36, 45, 72);
        clearAllButton.GlowColor = IndigoColor;
        heroPanel.AccentColor = Color.FromArgb(80, 120, 255);
        keyRailPanel.AccentColor = Color.FromArgb(40, 180, 255);
        textPanel.AccentColor = Color.FromArgb(60, 200, 190);
        imagePanel.AccentColor = Color.FromArgb(90, 110, 255);
        previewPanel.AccentColor = Color.FromArgb(80, 120, 255);

        heroPanel.Padding = new Padding(0);
        brandPictureBox.Padding = new Padding(0);
    }

    private void InitializeStarfield()
    {
        CreateStars();
        starfieldTimer.Tick += (_, _) =>
        {
            AdvanceStars();
            AdvanceShootingStars();
            Invalidate();
        };
        starfieldTimer.Start();
        Resize += (_, _) => CreateStars();
    }

    private void CreateStars()
    {
        stars.Clear();

        if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
        {
            return;
        }

        int starCount = Math.Max(70, (ClientSize.Width * ClientSize.Height) / 12000);
        for (int index = 0; index < starCount; index++)
        {
            stars.Add(CreateStar(random.NextSingle() * ClientSize.Width, random.NextSingle() * ClientSize.Height));
        }

        shootingStars.Clear();
    }

    private Star CreateStar(float x, float y)
    {
        float depth = 0.45F + (random.NextSingle() * 1.35F);
        return new Star(
            x,
            y,
            depth,
            0.6F + (depth * 1.5F),
            65 + random.Next(125),
            random.NextSingle() * MathF.Tau);
    }

    private void AdvanceStars()
    {
        if (stars.Count == 0)
        {
            return;
        }

        for (int index = 0; index < stars.Count; index++)
        {
            Star star = stars[index];
            float nextY = star.Y + (0.18F * star.Depth);
            float drift = MathF.Sin(star.Phase) * 0.12F * star.Depth;
            float nextX = star.X + drift;
            float nextPhase = star.Phase + (0.035F * star.Depth);

            if (nextY > ClientSize.Height + 6)
            {
                stars[index] = CreateStar(random.NextSingle() * ClientSize.Width, -8F);
                continue;
            }

            if (nextX < -6F)
            {
                nextX = ClientSize.Width + 6F;
            }
            else if (nextX > ClientSize.Width + 6F)
            {
                nextX = -6F;
            }

            stars[index] = star with { X = nextX, Y = nextY, Phase = nextPhase };
        }

        nebulaPhase += 0.0055F;
    }

    private void AdvanceShootingStars()
    {
        if (shootingStars.Count < 3 && random.NextDouble() < 0.028)
        {
            shootingStars.Add(new ShootingStar(
                random.NextSingle() * ClientSize.Width * 0.8F,
                random.NextSingle() * ClientSize.Height * 0.35F,
                5.5F + (random.NextSingle() * 3.4F),
                2.5F + (random.NextSingle() * 1.6F),
                90F + (random.NextSingle() * 35F),
                130 + random.Next(90)));
        }

        for (int index = shootingStars.Count - 1; index >= 0; index--)
        {
            ShootingStar star = shootingStars[index];
            float nextX = star.X + star.Speed;
            float nextY = star.Y + (star.Speed * 0.46F);
            float nextLife = star.Life - 2.2F;

            if (nextLife <= 0 || nextX > ClientSize.Width + 140 || nextY > ClientSize.Height + 140)
            {
                shootingStars.RemoveAt(index);
                continue;
            }

            shootingStars[index] = star with { X = nextX, Y = nextY, Life = nextLife };
        }
    }

    private void ConfigureDragAndDrop()
    {
        AllowDrop = true;
        imagePanel.AllowDrop = true;
        previewPanel.AllowDrop = true;
        imagePreviewBox.AllowDrop = true;

        DragEnter += SharedDragEnter;
        imagePanel.DragEnter += SharedDragEnter;
        previewPanel.DragEnter += SharedDragEnter;
        imagePreviewBox.DragEnter += SharedDragEnter;

        DragDrop += SharedDragDrop;
        imagePanel.DragDrop += SharedDragDrop;
        previewPanel.DragDrop += SharedDragDrop;
        imagePreviewBox.DragDrop += SharedDragDrop;
    }

    private void ConfigureButton(NeonButton button, Color color, bool isSecondary = false)
    {
        button.FillColor = color;
        button.GlowColor = color;
        button.IsSecondary = isSecondary;
    }

    private void LoadBrandImage()
    {
        string imagePath = Path.Combine(AppContext.BaseDirectory, "magsec_logo_transparent.png");
        if (!File.Exists(imagePath))
        {
            imagePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "magsec_logo_transparent.png"));
        }

        if (File.Exists(imagePath))
        {
            using Image original = Image.FromFile(imagePath);
            brandPictureBox.Image = new Bitmap(original);
        }
    }

    private void StyleTextBox(TextBox textBox)
    {
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.BackColor = PanelColor;
        textBox.ForeColor = TextMainColor;
        textBox.Font = textBox.Multiline
            ? new Font("Segoe UI", 10.75F, FontStyle.Regular)
            : new Font("Segoe UI Semibold", 10.25F, FontStyle.Regular);
    }

    private void StyleTextBox(StyledEditor editor)
    {
        editor.Font = new Font("Segoe UI", 10.75F, FontStyle.Regular);
    }

    private void EncryptTextButton_Click(object sender, EventArgs e)
    {
        try
        {
            string secret = BuildSecret();
            if (string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                ShowStatus("Enter text to encrypt.", isError: true);
                return;
            }

            byte[] encrypted = EncryptBytes(Encoding.UTF8.GetBytes(inputTextBox.Text), secret, TextPrefix);
            outputTextBox.Text = Convert.ToBase64String(encrypted);
            ShowStatus("Text encrypted successfully.", isError: false);
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message, isError: true);
        }
    }

    private void DecryptTextButton_Click(object sender, EventArgs e)
    {
        try
        {
            string secret = BuildSecret();
            if (string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                ShowStatus("Enter encrypted text to decrypt.", isError: true);
                return;
            }

            byte[] payload = Convert.FromBase64String(inputTextBox.Text.Trim());
            byte[] plain = DecryptBytes(payload, secret, TextPrefix, LegacyTextPrefix);
            outputTextBox.Text = Encoding.UTF8.GetString(plain);
            ShowStatus("Text decrypted successfully.", isError: false);
        }
        catch (FormatException)
        {
            ShowStatus("The encrypted text is not valid Base64.", isError: true);
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message, isError: true);
        }
    }

    private void CopyTextButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(outputTextBox.Text))
        {
            ShowStatus("There is no output to copy yet.", isError: true);
            return;
        }

        Clipboard.SetText(outputTextBox.Text);
        ShowStatus("Output copied.", isError: false);
    }

    private void ClearAllButton_Click(object sender, EventArgs e)
    {
        inputTextBox.Clear();
        outputTextBox.Clear();
        key1TextBox.Clear();
        key2TextBox.Clear();
        key3TextBox.Clear();
        key4TextBox.Clear();
        selectedImagePath = null;
        selectedEncryptedImagePath = null;
        imagePreviewBox.Image = null;
        imagePathValueLabel.Text = "No image selected";
        encryptedImagePathValueLabel.Text = "No encrypted file selected";
        ShowStatus("Workspace cleared.", isError: false);
    }

    private void GenerateKeyButton_Click(object sender, EventArgs e)
    {
        TextBox target = new[] { key1TextBox, key2TextBox, key3TextBox, key4TextBox }
            .FirstOrDefault(box => string.IsNullOrWhiteSpace(box.Text)) ?? key1TextBox;

        target.Text = GenerateStrongKey(24);

        if (!keysVisible)
        {
            SetKeysVisible(true);
        }

        target.Focus();
        ShowStatus("Strong key generated. Store it somewhere safe.", isError: false);
    }

    private void ToggleKeysButton_Click(object sender, EventArgs e)
    {
        SetKeysVisible(!keysVisible);
    }

    private void SetKeysVisible(bool visible)
    {
        keysVisible = visible;
        char passwordChar = visible ? '\0' : '*';
        foreach (TextBox keyBox in new[] { key1TextBox, key2TextBox, key3TextBox, key4TextBox })
        {
            keyBox.PasswordChar = passwordChar;
        }

        toggleKeysButton.Text = visible ? "Hide keys" : "Show keys";
    }

    private static string GenerateStrongKey(int length)
    {
        // Unambiguous alphabet (no 0/O/1/l/I) so generated keys are easy to transcribe.
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%^&*-_=+";
        char[] result = new char[length];
        for (int index = 0; index < length; index++)
        {
            result[index] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
        }

        return new string(result);
    }

    private void UpdateKeyStrength()
    {
        int totalLength = new[] { key1TextBox.Text, key2TextBox.Text, key3TextBox.Text, key4TextBox.Text }
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Sum(key => key.Trim().Length);

        (string label, Color color) = totalLength switch
        {
            0 => ("Strength: —", TextMutedColor),
            < MinimumKeyLength => ("Strength: Weak", DangerColor),
            < 16 => ("Strength: Fair", Color.FromArgb(250, 204, 21)),
            < 28 => ("Strength: Strong", EmeraldColor),
            _ => ("Strength: Excellent", TealColor)
        };

        keyStrengthLabel.Text = label;
        keyStrengthLabel.ForeColor = color;
    }

    private void SelectImageButton_Click(object sender, EventArgs e)
    {
        using OpenFileDialog dialog = new()
        {
            Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp|All files|*.*",
            Title = "Select an image"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadSelectedImage(dialog.FileName);
        }
    }

    private void EncryptImageButton_Click(object sender, EventArgs e)
    {
        try
        {
            string secret = BuildSecret();
            if (string.IsNullOrWhiteSpace(selectedImagePath) || !File.Exists(selectedImagePath))
            {
                ShowStatus("Select an image before encrypting.", isError: true);
                return;
            }

            using SaveFileDialog dialog = new()
            {
                Filter = "MagSec encrypted image|*.mseimg",
                FileName = $"{Path.GetFileNameWithoutExtension(selectedImagePath)}.mseimg",
                Title = "Save encrypted image"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            byte[] fileBytes = File.ReadAllBytes(selectedImagePath);
            byte[] imagePackage = BuildImagePackage(Path.GetExtension(selectedImagePath), fileBytes);
            byte[] encrypted = EncryptBytes(imagePackage, secret, ImagePrefix);
            File.WriteAllBytes(dialog.FileName, encrypted);

            selectedEncryptedImagePath = dialog.FileName;
            encryptedImagePathValueLabel.Text = Path.GetFileName(dialog.FileName);
            ShowStatus("Image encrypted and saved.", isError: false);
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message, isError: true);
        }
    }

    private void SelectEncryptedImageButton_Click(object sender, EventArgs e)
    {
        using OpenFileDialog dialog = new()
        {
            Filter = "MagSec encrypted image|*.mseimg|All files|*.*",
            Title = "Select an encrypted image"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadEncryptedImage(dialog.FileName);
        }
    }

    private void DecryptImageButton_Click(object sender, EventArgs e)
    {
        try
        {
            string secret = BuildSecret();
            if (string.IsNullOrWhiteSpace(selectedEncryptedImagePath) || !File.Exists(selectedEncryptedImagePath))
            {
                ShowStatus("Select an encrypted image before decrypting.", isError: true);
                return;
            }

            byte[] encrypted = File.ReadAllBytes(selectedEncryptedImagePath);
            byte[] decryptedPackage = DecryptBytes(encrypted, secret, ImagePrefix, LegacyImagePrefix);
            (string extension, byte[] imageBytes) = ReadImagePackage(decryptedPackage);

            using SaveFileDialog dialog = new()
            {
                Filter = $"Original file|*{extension}|All files|*.*",
                FileName = $"{Path.GetFileNameWithoutExtension(selectedEncryptedImagePath)}_restored{extension}",
                Title = "Save decrypted image"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(dialog.FileName, imageBytes);
            imagePreviewBox.Image?.Dispose();
            using MemoryStream stream = new(imageBytes);
            using Image restored = Image.FromStream(stream);
            imagePreviewBox.Image = new Bitmap(restored);
            imagePathValueLabel.Text = Path.GetFileName(dialog.FileName);
            ShowStatus("Image decrypted and saved.", isError: false);
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message, isError: true);
        }
    }

    private string BuildSecret()
    {
        string[] keys =
        [
            key1TextBox.Text.Trim(),
            key2TextBox.Text.Trim(),
            key3TextBox.Text.Trim(),
            key4TextBox.Text.Trim()
        ];

        string[] filledKeys = keys.Where(key => !string.IsNullOrWhiteSpace(key)).ToArray();
        if (filledKeys.Length == 0)
        {
            throw new InvalidOperationException("Enter at least one key.");
        }

        if (filledKeys.Sum(key => key.Length) < MinimumKeyLength)
        {
            throw new InvalidOperationException($"Use at least {MinimumKeyLength} characters across your keys for a secure result.");
        }

        return string.Join("|", filledKeys);
    }

    private static byte[] EncryptBytes(byte[] plainBytes, string secret, string prefix)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] key = DeriveKey(secret, salt, Pbkdf2Iterations, KdfSha512);

        byte[] payload = new byte[V2HeaderLength + TagSize + plainBytes.Length];
        Encoding.ASCII.GetBytes(prefix).CopyTo(payload, 0);
        payload[4] = FormatVersion;
        payload[5] = KdfSha512;
        BinaryPrimitives.WriteInt32BigEndian(payload.AsSpan(6, 4), Pbkdf2Iterations);
        salt.CopyTo(payload, 10);
        nonce.CopyTo(payload, 10 + SaltSize);

        // The whole header (prefix, version, KDF params, salt, nonce) is bound into the
        // authentication tag as associated data so tampering with it is detected on decrypt.
        ReadOnlySpan<byte> associatedData = payload.AsSpan(0, V2HeaderLength);
        Span<byte> tag = payload.AsSpan(V2HeaderLength, TagSize);
        Span<byte> cipherBytes = payload.AsSpan(V2HeaderLength + TagSize);

        try
        {
            using var aes = new AesGcm(key, TagSize);
            aes.Encrypt(nonce, plainBytes, cipherBytes, tag, associatedData);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
        }

        return payload;
    }

    private static byte[] DecryptBytes(byte[] payload, string secret, string expectedPrefix, string legacyPrefix)
    {
        if (payload.Length < 4)
        {
            throw new InvalidOperationException("The encrypted content is incomplete.");
        }

        string prefix = Encoding.ASCII.GetString(payload, 0, 4);
        if (string.Equals(prefix, expectedPrefix, StringComparison.Ordinal))
        {
            return DecryptV2(payload, secret);
        }

        if (string.Equals(prefix, legacyPrefix, StringComparison.Ordinal))
        {
            return DecryptLegacy(payload, secret);
        }

        throw new InvalidOperationException("Unknown encryption format.");
    }

    private static byte[] DecryptV2(byte[] payload, string secret)
    {
        if (payload.Length < V2HeaderLength + TagSize)
        {
            throw new InvalidOperationException("The encrypted content is incomplete.");
        }

        byte version = payload[4];
        if (version != FormatVersion)
        {
            throw new InvalidOperationException("Unsupported encryption version.");
        }

        byte kdfId = payload[5];
        int iterations = BinaryPrimitives.ReadInt32BigEndian(payload.AsSpan(6, 4));
        if (iterations < 1 || iterations > 10_000_000)
        {
            throw new InvalidOperationException("Invalid key derivation parameters.");
        }

        byte[] salt = payload[10..(10 + SaltSize)];
        byte[] nonce = payload[(10 + SaltSize)..V2HeaderLength];
        byte[] tag = payload[V2HeaderLength..(V2HeaderLength + TagSize)];
        byte[] cipherBytes = payload[(V2HeaderLength + TagSize)..];
        byte[] plainBytes = new byte[cipherBytes.Length];
        byte[] key = DeriveKey(secret, salt, iterations, kdfId);

        try
        {
            using var aes = new AesGcm(key, TagSize);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes, payload.AsSpan(0, V2HeaderLength));
        }
        catch (CryptographicException)
        {
            CryptographicOperations.ZeroMemory(plainBytes);
            throw new InvalidOperationException("The keys do not match or the data is corrupted.");
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
        }

        return plainBytes;
    }

    private static byte[] DecryptLegacy(byte[] payload, string secret)
    {
        if (payload.Length <= LegacyHeaderLength)
        {
            throw new InvalidOperationException("The encrypted content is incomplete.");
        }

        byte[] salt = payload[4..(4 + SaltSize)];
        byte[] nonce = payload[(4 + SaltSize)..(4 + SaltSize + NonceSize)];
        byte[] tag = payload[(4 + SaltSize + NonceSize)..LegacyHeaderLength];
        byte[] cipherBytes = payload[LegacyHeaderLength..];
        byte[] plainBytes = new byte[cipherBytes.Length];
        byte[] key = DeriveKey(secret, salt, LegacyPbkdf2Iterations, KdfSha256);

        try
        {
            using var aes = new AesGcm(key, TagSize);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
        }
        catch (CryptographicException)
        {
            CryptographicOperations.ZeroMemory(plainBytes);
            throw new InvalidOperationException("The keys do not match or the data is corrupted.");
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
        }

        return plainBytes;
    }

    private static byte[] BuildImagePackage(string extension, byte[] imageBytes)
    {
        byte[] extensionBytes = Encoding.UTF8.GetBytes(extension);
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);
        writer.Write((ushort)extensionBytes.Length);
        writer.Write(extensionBytes);
        writer.Write(imageBytes.Length);
        writer.Write(imageBytes);
        writer.Flush();
        return stream.ToArray();
    }

    private static (string extension, byte[] imageBytes) ReadImagePackage(byte[] package)
    {
        using MemoryStream stream = new(package);
        using BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen: true);
        int extensionLength = reader.ReadUInt16();
        string extension = Encoding.UTF8.GetString(reader.ReadBytes(extensionLength));
        int imageLength = reader.ReadInt32();
        byte[] imageBytes = reader.ReadBytes(imageLength);

        if (imageBytes.Length != imageLength)
        {
            throw new InvalidOperationException("The encrypted image file is incomplete.");
        }

        return (extension, imageBytes);
    }

    private static byte[] DeriveKey(string secret, byte[] salt, int iterations, byte kdfId)
    {
        HashAlgorithmName hash = kdfId == KdfSha512 ? HashAlgorithmName.SHA512 : HashAlgorithmName.SHA256;
        byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

        try
        {
            return Rfc2898DeriveBytes.Pbkdf2(secretBytes, salt, iterations, hash, KeySize);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(secretBytes);
        }
    }

    private void ShowStatus(string message, bool isError)
    {
        heroMetaLabel.Text = message;
        heroMetaLabel.ForeColor = isError ? Color.FromArgb(255, 205, 214) : Color.FromArgb(176, 230, 230, 240);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using LinearGradientBrush backgroundBrush = new(
            ClientRectangle,
            Color.FromArgb(4, 8, 22),
            Color.FromArgb(9, 16, 38),
            90F);
        e.Graphics.FillRectangle(backgroundBrush, ClientRectangle);

        Rectangle upperGlow = new(-180, -120, 580, 380);
        using GraphicsPath upperPath = new();
        upperPath.AddEllipse(upperGlow);
        using PathGradientBrush upperBrush = new(upperPath)
        {
            CenterColor = Color.FromArgb(58 + (int)(18 * MathF.Sin(nebulaPhase)), 56, 189, 248),
            SurroundColors = [Color.Transparent]
        };
        e.Graphics.FillEllipse(upperBrush, upperGlow);

        Rectangle lowerGlow = new(ClientSize.Width - 430, ClientSize.Height - 340, 500, 320);
        using GraphicsPath lowerPath = new();
        lowerPath.AddEllipse(lowerGlow);
        using PathGradientBrush lowerBrush = new(lowerPath)
        {
            CenterColor = Color.FromArgb(44 + (int)(14 * MathF.Cos(nebulaPhase * 1.2F)), 94, 92, 230),
            SurroundColors = [Color.Transparent]
        };
        e.Graphics.FillEllipse(lowerBrush, lowerGlow);

        Rectangle centerGlow = new((ClientSize.Width / 2) - 280, -40, 560, 220);
        using GraphicsPath centerPath = new();
        centerPath.AddEllipse(centerGlow);
        using PathGradientBrush centerBrush = new(centerPath)
        {
            CenterColor = Color.FromArgb(24 + (int)(10 * MathF.Sin(nebulaPhase * 1.8F)), 120, 160, 255),
            SurroundColors = [Color.Transparent]
        };
        e.Graphics.FillEllipse(centerBrush, centerGlow);

        foreach (Star star in stars)
        {
            int alpha = (int)(star.Alpha * (0.65F + (0.35F * MathF.Sin(star.Phase))));
            using SolidBrush starBrush = new(Color.FromArgb(alpha, 235, 244, 255));
            e.Graphics.FillEllipse(starBrush, star.X, star.Y, star.Size, star.Size);

            if (star.Depth > 1.35F)
            {
                using Pen trailPen = new(Color.FromArgb(alpha / 3, 145, 220, 255), 1F);
                e.Graphics.DrawLine(trailPen, star.X + (star.Size / 2F), star.Y - (star.Size * 1.8F), star.X + (star.Size / 2F), star.Y);
            }
        }

        foreach (ShootingStar shootingStar in shootingStars)
        {
            using Pen trailPen = new(Color.FromArgb((int)shootingStar.Life, 210, 240, 255), shootingStar.Thickness)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
            float tailX = shootingStar.X - shootingStar.TailLength;
            float tailY = shootingStar.Y - (shootingStar.TailLength * 0.46F);
            e.Graphics.DrawLine(trailPen, tailX, tailY, shootingStar.X, shootingStar.Y);
        }
    }

    private void SharedDragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
            if (files.Length > 0 && IsSupportedDropFile(files[0]))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }
        }

        e.Effect = DragDropEffects.None;
    }

    private void SharedDragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) != true)
        {
            return;
        }

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop)!;
        if (files.Length == 0)
        {
            return;
        }

        string filePath = files[0];
        if (string.Equals(Path.GetExtension(filePath), ".mseimg", StringComparison.OrdinalIgnoreCase))
        {
            LoadEncryptedImage(filePath);
        }
        else
        {
            LoadSelectedImage(filePath);
        }
    }

    private static bool IsSupportedDropFile(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension is ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".webp" or ".mseimg";
    }

    private void LoadSelectedImage(string filePath)
    {
        selectedImagePath = filePath;
        imagePathValueLabel.Text = Path.GetFileName(filePath);
        imagePreviewBox.Image?.Dispose();
        using Image original = Image.FromFile(filePath);
        imagePreviewBox.Image = new Bitmap(original);
        ShowStatus("Image loaded.", isError: false);
    }

    private void LoadEncryptedImage(string filePath)
    {
        selectedEncryptedImagePath = filePath;
        encryptedImagePathValueLabel.Text = Path.GetFileName(filePath);
        ShowStatus("Encrypted image loaded.", isError: false);
    }

    private sealed record Star(float X, float Y, float Depth, float Size, int Alpha, float Phase);
    private sealed record ShootingStar(float X, float Y, float Speed, float Thickness, float TailLength, float Life);

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        starfieldTimer.Stop();
        starfieldTimer.Dispose();
        base.OnFormClosed(e);
    }
}

internal sealed class GlassPanel : Panel
{
    private Color accentColor = Color.FromArgb(14, 165, 233);

    [DefaultValue(typeof(Color), "14, 165, 233")]
    public Color AccentColor
    {
        get => accentColor;
        set
        {
            accentColor = value;
            Invalidate();
        }
    }

    public GlassPanel()
    {
        BackColor = Color.Transparent;
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle rect = new(0, 0, Width - 1, Height - 1);

        using GraphicsPath path = CreateRoundedRectangle(rect, 18);
        Rectangle shineRect = new(0, 0, Width, Math.Max(42, Height / 2));
        using LinearGradientBrush fillBrush = new(
            rect,
            Color.FromArgb(44, 16, 24, 46),
            Color.FromArgb(32, 11, 16, 34),
            90F);
        using GraphicsPath shinePath = CreateRoundedRectangle(shineRect, 18);
        using PathGradientBrush shineBrush = new(shinePath)
        {
            CenterColor = Color.FromArgb(34, accentColor),
            SurroundColors = [Color.Transparent]
        };
        using Pen borderPen = new(Color.FromArgb(54, 255, 255, 255));
        using Pen accentPen = new(Color.FromArgb(110, accentColor), 1.15F);

        e.Graphics.FillPath(fillBrush, path);
        e.Graphics.FillPath(shineBrush, shinePath);
        e.Graphics.DrawPath(borderPen, path);
        e.Graphics.DrawArc(accentPen, 12, 10, Math.Max(56, Width / 4), 24, 200, 120);

        base.OnPaint(e);
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        int diameter = radius * 2;
        GraphicsPath path = new();
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}

internal sealed class StyledEditor : UserControl
{
    private readonly RichTextBox editor;
    private readonly SlimScrollBar scrollBar;
    private Color surfaceColor = Color.FromArgb(12, 17, 39);
    private Color borderColor = Color.FromArgb(62, 120, 140, 176);

    public StyledEditor()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        DoubleBuffered = true;
        BackColor = Color.Transparent;
        Padding = new Padding(1);

        editor = new RichTextBox
        {
            BorderStyle = BorderStyle.None,
            Dock = DockStyle.Fill,
            ScrollBars = RichTextBoxScrollBars.None,
            Multiline = true,
            WordWrap = true,
            DetectUrls = false,
            AcceptsTab = true
        };

        scrollBar = new SlimScrollBar
        {
            Dock = DockStyle.Right,
            Width = 14
        };

        Controls.Add(editor);
        Controls.Add(scrollBar);

        editor.VScroll += (_, _) => SyncScrollBar();
        editor.TextChanged += (_, _) => SyncScrollBar();
        editor.Resize += (_, _) => SyncScrollBar();
        editor.SelectionChanged += (_, _) => SyncScrollBar();
        editor.MouseWheel += (_, _) => BeginInvoke(new Action(SyncScrollBar));
        editor.KeyUp += (_, _) => BeginInvoke(new Action(SyncScrollBar));
        scrollBar.ValueChanged += (_, _) => ScrollToBarValue();
    }

    public override string Text
    {
        get => editor.Text;
        set
        {
            editor.Text = value;
            SyncScrollBar();
        }
    }

    public override Font Font
    {
        get => editor.Font;
        set => editor.Font = value;
    }

    public override Color ForeColor
    {
        get => editor.ForeColor;
        set => editor.ForeColor = value;
    }

    public override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    public void Clear()
    {
        editor.Clear();
        SyncScrollBar();
    }

    public void ApplyTheme(Color editorBackColor, Color editorForeColor, Color trackColor, Color thumbColor)
    {
        surfaceColor = editorBackColor;
        editor.BackColor = editorBackColor;
        editor.ForeColor = editorForeColor;
        borderColor = Color.FromArgb(72, ControlPaint.Light(trackColor, 0.35F));
        scrollBar.TrackColor = Color.FromArgb(38, trackColor);
        scrollBar.TrackGlowColor = Color.FromArgb(26, thumbColor);
        scrollBar.ThumbColor = thumbColor;
        scrollBar.ThumbHighlightColor = ControlPaint.Light(thumbColor, 0.16F);
        Invalidate();
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        SyncScrollBar();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle rect = new(0, 0, Width - 1, Height - 1);
        using GraphicsPath path = CreateRoundedRectangle(rect, 14);
        using LinearGradientBrush fillBrush = new(
            rect,
            Color.FromArgb(238, surfaceColor),
            Color.FromArgb(228, ControlPaint.Dark(surfaceColor, 0.06F)),
            90F);
        using Pen borderPen = new(borderColor);
        e.Graphics.FillPath(fillBrush, path);
        e.Graphics.DrawPath(borderPen, path);
        base.OnPaint(e);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        SyncScrollBar();
    }

    private void SyncScrollBar()
    {
        if (!IsHandleCreated || !editor.IsHandleCreated)
        {
            return;
        }

        int lineCount = Math.Max(1, editor.GetLineFromCharIndex(editor.TextLength) + 1);
        int visibleLines = Math.Max(1, editor.ClientSize.Height / Math.Max(1, TextRenderer.MeasureText("Ag", editor.Font).Height));
        int maxOffset = Math.Max(0, lineCount - visibleLines);
        int firstVisibleLine = GetFirstVisibleLine(editor.Handle);

        scrollBar.ViewSize = visibleLines;
        scrollBar.Maximum = lineCount;
        scrollBar.Enabled = maxOffset > 0;
        scrollBar.SetValueSilently(Math.Min(firstVisibleLine, maxOffset));
    }

    private void ScrollToBarValue()
    {
        if (!editor.IsHandleCreated)
        {
            return;
        }

        int firstVisibleLine = GetFirstVisibleLine(editor.Handle);
        int delta = scrollBar.Value - firstVisibleLine;
        if (delta != 0)
        {
            SendMessage(editor.Handle, EmLineScroll, IntPtr.Zero, (IntPtr)delta);
        }
    }

    private static int GetFirstVisibleLine(IntPtr handle)
    {
        return SendMessage(handle, EmGetFirstVisibleLine, IntPtr.Zero, IntPtr.Zero).ToInt32();
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        int diameter = radius * 2;
        GraphicsPath path = new();
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private const int EmGetFirstVisibleLine = 0x00CE;
    private const int EmLineScroll = 0x00B6;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}

internal sealed class SlimScrollBar : Control
{
    private const int MinimumThumbHeight = 36;
    private int maximum = 1;
    private int value;
    private int viewSize = 1;
    private bool dragging;
    private bool hovered;
    private int dragOffset;

    public SlimScrollBar()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        DoubleBuffered = true;
        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        Width = 14;
    }

    public event EventHandler? ValueChanged;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color TrackColor { get; set; } = Color.FromArgb(18, 28, 52);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color TrackGlowColor { get; set; } = Color.FromArgb(22, 95, 108, 255);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color ThumbColor { get; set; } = Color.FromArgb(95, 108, 255);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color ThumbHighlightColor { get; set; } = Color.FromArgb(128, 148, 160, 255);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Maximum
    {
        get => maximum;
        set
        {
            maximum = Math.Max(1, value);
            Invalidate();
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ViewSize
    {
        get => viewSize;
        set
        {
            viewSize = Math.Max(1, value);
            Invalidate();
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Value
    {
        get => value;
        set => SetValue(value, true);
    }

    public void SetValueSilently(int newValue)
    {
        SetValue(newValue, false);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Color.Transparent);

        Rectangle trackRect = new(3, 6, Width - 6, Height - 12);
        using GraphicsPath trackPath = CreateRoundedRectangle(trackRect, 4);
        using SolidBrush trackBrush = new(Enabled ? TrackColor : Color.FromArgb(10, TrackColor));
        e.Graphics.FillPath(trackBrush, trackPath);

        Rectangle glowRect = new(trackRect.X - 1, trackRect.Y, trackRect.Width + 2, trackRect.Height);
        using GraphicsPath glowPath = CreateRoundedRectangle(glowRect, 5);
        using PathGradientBrush glowBrush = new(glowPath)
        {
            CenterColor = Enabled ? TrackGlowColor : Color.Transparent,
            SurroundColors = [Color.Transparent]
        };
        e.Graphics.FillPath(glowBrush, glowPath);

        if (!Enabled)
        {
            return;
        }

        Rectangle thumbRect = GetThumbRectangle(trackRect);
        using GraphicsPath thumbPath = CreateRoundedRectangle(thumbRect, 4);
        using LinearGradientBrush thumbBrush = new(
            thumbRect,
            hovered || dragging ? ControlPaint.Light(ThumbHighlightColor, 0.06F) : ThumbHighlightColor,
            hovered || dragging ? ControlPaint.Light(ThumbColor, 0.02F) : ThumbColor,
            90F);
        using Pen thumbBorderPen = new(Color.FromArgb(110, ControlPaint.Light(ThumbColor, 0.18F)));
        e.Graphics.FillPath(thumbBrush, thumbPath);
        e.Graphics.DrawPath(thumbBorderPen, thumbPath);
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (!Enabled)
        {
            return;
        }

        Rectangle trackRect = new(3, 6, Width - 6, Height - 12);
        Rectangle thumbRect = GetThumbRectangle(trackRect);

        if (thumbRect.Contains(e.Location))
        {
            dragging = true;
            dragOffset = e.Y - thumbRect.Y;
            return;
        }

        int pageStep = Math.Max(1, ViewSize - 1);
        Value += e.Y < thumbRect.Y ? -pageStep : pageStep;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (Enabled)
        {
            Rectangle trackRect = new(3, 6, Width - 6, Height - 12);
            bool isHovering = GetThumbRectangle(trackRect).Contains(e.Location);
            if (hovered != isHovering)
            {
                hovered = isHovering;
                Invalidate();
            }
        }

        if (!dragging || !Enabled)
        {
            return;
        }

        Rectangle dragTrackRect = new(3, 6, Width - 6, Height - 12);
        int thumbHeight = GetThumbHeight(dragTrackRect.Height);
        int movableHeight = Math.Max(1, dragTrackRect.Height - thumbHeight);
        int top = Math.Clamp(e.Y - dragOffset - dragTrackRect.Y, 0, movableHeight);
        float ratio = top / (float)movableHeight;
        int maxValue = Math.Max(0, Maximum - ViewSize);
        Value = (int)Math.Round(ratio * maxValue);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        dragging = false;
        Invalidate();
        base.OnMouseUp(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        hovered = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    private void SetValue(int newValue, bool raiseEvent)
    {
        int maxValue = Math.Max(0, Maximum - ViewSize);
        int clamped = Math.Clamp(newValue, 0, maxValue);
        if (this.value == clamped)
        {
            return;
        }

        this.value = clamped;
        Invalidate();

        if (raiseEvent)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private Rectangle GetThumbRectangle(Rectangle trackRect)
    {
        int thumbHeight = GetThumbHeight(trackRect.Height);
        int movableHeight = Math.Max(1, trackRect.Height - thumbHeight);
        int maxValue = Math.Max(1, Maximum - ViewSize);
        int top = (int)Math.Round((Value / (float)maxValue) * movableHeight);
        return new Rectangle(trackRect.X, trackRect.Y + top, trackRect.Width, thumbHeight);
    }

    private int GetThumbHeight(int trackHeight)
    {
        float ratio = Math.Clamp(ViewSize / (float)Math.Max(ViewSize, Maximum), 0.1F, 1F);
        return Math.Max(MinimumThumbHeight, (int)Math.Round(trackHeight * ratio));
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        int diameter = radius * 2;
        GraphicsPath path = new();
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}

internal sealed class NeonButton : Button
{
    private Color fillColor = Color.FromArgb(14, 165, 233);
    private Color glowColor = Color.FromArgb(14, 165, 233);
    private bool isSecondary;
    private bool isHovered;
    private bool isPressed;

    [DefaultValue(typeof(Color), "14, 165, 233")]
    public Color FillColor
    {
        get => fillColor;
        set
        {
            fillColor = value;
            Invalidate();
        }
    }

    [DefaultValue(typeof(Color), "14, 165, 233")]
    public Color GlowColor
    {
        get => glowColor;
        set
        {
            glowColor = value;
            Invalidate();
        }
    }

    [DefaultValue(false)]
    public bool IsSecondary
    {
        get => isSecondary;
        set
        {
            isSecondary = value;
            Invalidate();
        }
    }

    public NeonButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        ForeColor = Color.White;
        Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        Cursor = Cursors.Hand;
        DoubleBuffered = true;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        isHovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        isHovered = false;
        isPressed = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        isPressed = true;
        Invalidate();
        base.OnMouseDown(mevent);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        isPressed = false;
        Invalidate();
        base.OnMouseUp(mevent);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Color.Transparent);

        Rectangle rect = new(0, 0, Width - 1, Height - 1);
        using GraphicsPath path = CreateRoundedRectangle(rect, 14);
        Color baseColor = FillColor;
        Color currentColor = isPressed
            ? ControlPaint.Dark(baseColor, 0.14F)
            : isHovered ? ControlPaint.Light(baseColor, 0.08F) : baseColor;

        using LinearGradientBrush fillBrush = new(
            rect,
            ControlPaint.Light(currentColor, isSecondary ? 0.02F : 0.08F),
            ControlPaint.Dark(currentColor, 0.06F),
            90F);
        using Pen borderPen = new(IsSecondary ? Color.FromArgb(80, 148, 163, 184) : Color.FromArgb(80, GlowColor), 1.1F);
        e.Graphics.FillPath(fillBrush, path);
        e.Graphics.DrawPath(borderPen, path);

        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            rect,
            ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        int diameter = radius * 2;
        GraphicsPath path = new();
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}
