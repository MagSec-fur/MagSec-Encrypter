namespace TextEncryptorApp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        heroPanel = new GlassPanel();
        heroMetaLabel = new Label();
        subtitleLabel = new Label();
        titleLabel = new Label();
        heroKickerLabel = new Label();
        brandPictureBox = new PictureBox();
        keyRailPanel = new GlassPanel();
        sideInfoLabel = new Label();
        key4TextBox = new TextBox();
        key4Label = new Label();
        key3TextBox = new TextBox();
        key3Label = new Label();
        key2TextBox = new TextBox();
        key2Label = new Label();
        key1TextBox = new TextBox();
        key1Label = new Label();
        railDescriptionLabel = new Label();
        railTitleLabel = new Label();
        textPanel = new GlassPanel();
        clearAllButton = new NeonButton();
        copyTextButton = new NeonButton();
        decryptTextButton = new NeonButton();
        encryptTextButton = new NeonButton();
        outputTextBox = new StyledEditor();
        outputLabel = new Label();
        inputTextBox = new StyledEditor();
        inputLabel = new Label();
        textPanelDescription = new Label();
        imagePanel = new GlassPanel();
        imageDropHintLabel = new Label();
        decryptImageButton = new NeonButton();
        selectEncryptedImageButton = new NeonButton();
        encryptImageButton = new NeonButton();
        selectImageButton = new NeonButton();
        encryptedImagePathValueLabel = new Label();
        encryptedImagePathLabel = new Label();
        imagePathValueLabel = new Label();
        imagePathLabel = new Label();
        previewPanel = new GlassPanel();
        imagePreviewBox = new PictureBox();
        imagePanelDescription = new Label();
        imagePanelTitle = new Label();
        heroPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)brandPictureBox).BeginInit();
        keyRailPanel.SuspendLayout();
        textPanel.SuspendLayout();
        imagePanel.SuspendLayout();
        previewPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)imagePreviewBox).BeginInit();
        SuspendLayout();
        // 
        // heroPanel
        // 
        heroPanel.Controls.Add(heroMetaLabel);
        heroPanel.Controls.Add(subtitleLabel);
        heroPanel.Controls.Add(titleLabel);
        heroPanel.Controls.Add(heroKickerLabel);
        heroPanel.Controls.Add(brandPictureBox);
        heroPanel.Location = new Point(24, 24);
        heroPanel.Name = "heroPanel";
        heroPanel.Size = new Size(1264, 144);
        heroPanel.TabIndex = 0;
        // 
        // heroMetaLabel
        // 
        heroMetaLabel.AutoSize = true;
        heroMetaLabel.Font = new Font("Segoe UI", 9F);
        heroMetaLabel.Location = new Point(1006, 95);
        heroMetaLabel.Name = "heroMetaLabel";
        heroMetaLabel.Size = new Size(170, 15);
        heroMetaLabel.TabIndex = 4;
        heroMetaLabel.Text = "Focused encryption workspace";
        // 
        // subtitleLabel
        // 
        subtitleLabel.AutoSize = true;
        subtitleLabel.Font = new Font("Segoe UI", 10F);
        subtitleLabel.ForeColor = Color.FromArgb(245, 248, 255);
        subtitleLabel.Location = new Point(173, 88);
        subtitleLabel.Name = "subtitleLabel";
        subtitleLabel.Size = new Size(547, 19);
        subtitleLabel.TabIndex = 3;
        subtitleLabel.Text = "Encrypt text and images in a clean workspace built for clarity, speed, and consistency.";
        // 
        // titleLabel
        // 
        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Century Gothic", 25F, FontStyle.Bold);
        titleLabel.ForeColor = Color.FromArgb(245, 248, 255);
        titleLabel.Location = new Point(171, 40);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(315, 40);
        titleLabel.TabIndex = 2;
        titleLabel.Text = "MagSec Encrypter";
        // 
        // heroKickerLabel
        // 
        heroKickerLabel.AutoSize = true;
        heroKickerLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        heroKickerLabel.ForeColor = Color.FromArgb(45, 212, 191);
        heroKickerLabel.Location = new Point(173, 23);
        heroKickerLabel.Name = "heroKickerLabel";
        heroKickerLabel.Size = new Size(131, 15);
        heroKickerLabel.TabIndex = 1;
        heroKickerLabel.Text = "SPACE CONSOLE";
        // 
        // brandPictureBox
        // 
        brandPictureBox.BackColor = Color.Transparent;
        brandPictureBox.Location = new Point(24, 22);
        brandPictureBox.Name = "brandPictureBox";
        brandPictureBox.Size = new Size(126, 98);
        brandPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        brandPictureBox.TabIndex = 0;
        brandPictureBox.TabStop = false;
        // 
        // keyRailPanel
        // 
        keyRailPanel.Controls.Add(sideInfoLabel);
        keyRailPanel.Controls.Add(key4TextBox);
        keyRailPanel.Controls.Add(key4Label);
        keyRailPanel.Controls.Add(key3TextBox);
        keyRailPanel.Controls.Add(key3Label);
        keyRailPanel.Controls.Add(key2TextBox);
        keyRailPanel.Controls.Add(key2Label);
        keyRailPanel.Controls.Add(key1TextBox);
        keyRailPanel.Controls.Add(key1Label);
        keyRailPanel.Controls.Add(railDescriptionLabel);
        keyRailPanel.Controls.Add(railTitleLabel);
        keyRailPanel.Location = new Point(24, 180);
        keyRailPanel.Name = "keyRailPanel";
        keyRailPanel.Size = new Size(282, 526);
        keyRailPanel.TabIndex = 1;
        // 
        // sideInfoLabel
        // 
        sideInfoLabel.Font = new Font("Segoe UI", 9F);
        sideInfoLabel.Location = new Point(24, 409);
        sideInfoLabel.Name = "sideInfoLabel";
        sideInfoLabel.Size = new Size(220, 56);
        sideInfoLabel.TabIndex = 10;
        sideInfoLabel.Text = "Use the same key combination for both text and image decryption later.";
        // 
        // key4TextBox
        // 
        key4TextBox.Location = new Point(24, 344);
        key4TextBox.Name = "key4TextBox";
        key4TextBox.PasswordChar = '*';
        key4TextBox.Size = new Size(220, 23);
        key4TextBox.TabIndex = 9;
        // 
        // key4Label
        // 
        key4Label.AutoSize = true;
        key4Label.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        key4Label.ForeColor = Color.FromArgb(245, 248, 255);
        key4Label.Location = new Point(24, 322);
        key4Label.Name = "key4Label";
        key4Label.Size = new Size(57, 17);
        key4Label.TabIndex = 8;
        key4Label.Text = "Key 4";
        // 
        // key3TextBox
        // 
        key3TextBox.Location = new Point(24, 282);
        key3TextBox.Name = "key3TextBox";
        key3TextBox.PasswordChar = '*';
        key3TextBox.Size = new Size(220, 23);
        key3TextBox.TabIndex = 7;
        // 
        // key3Label
        // 
        key3Label.AutoSize = true;
        key3Label.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        key3Label.ForeColor = Color.FromArgb(245, 248, 255);
        key3Label.Location = new Point(24, 260);
        key3Label.Name = "key3Label";
        key3Label.Size = new Size(57, 17);
        key3Label.TabIndex = 6;
        key3Label.Text = "Key 3";
        // 
        // key2TextBox
        // 
        key2TextBox.Location = new Point(24, 220);
        key2TextBox.Name = "key2TextBox";
        key2TextBox.PasswordChar = '*';
        key2TextBox.Size = new Size(220, 23);
        key2TextBox.TabIndex = 5;
        // 
        // key2Label
        // 
        key2Label.AutoSize = true;
        key2Label.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        key2Label.ForeColor = Color.FromArgb(245, 248, 255);
        key2Label.Location = new Point(24, 198);
        key2Label.Name = "key2Label";
        key2Label.Size = new Size(57, 17);
        key2Label.TabIndex = 4;
        key2Label.Text = "Key 2";
        // 
        // key1TextBox
        // 
        key1TextBox.Location = new Point(24, 158);
        key1TextBox.Name = "key1TextBox";
        key1TextBox.PasswordChar = '*';
        key1TextBox.Size = new Size(220, 23);
        key1TextBox.TabIndex = 3;
        // 
        // key1Label
        // 
        key1Label.AutoSize = true;
        key1Label.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        key1Label.ForeColor = Color.FromArgb(245, 248, 255);
        key1Label.Location = new Point(24, 136);
        key1Label.Name = "key1Label";
        key1Label.Size = new Size(68, 17);
        key1Label.TabIndex = 2;
        key1Label.Text = "Key 1 *";
        // 
        // railDescriptionLabel
        // 
        railDescriptionLabel.Font = new Font("Segoe UI", 9.5F);
        railDescriptionLabel.ForeColor = Color.FromArgb(148, 163, 184);
        railDescriptionLabel.Location = new Point(24, 52);
        railDescriptionLabel.Name = "railDescriptionLabel";
        railDescriptionLabel.Size = new Size(220, 38);
        railDescriptionLabel.TabIndex = 1;
        railDescriptionLabel.Text = "Four simple key fields for every encryption action.";
        // 
        // railTitleLabel
        // 
        railTitleLabel.AutoSize = true;
        railTitleLabel.Font = new Font("Bahnschrift SemiBold", 17F, FontStyle.Bold);
        railTitleLabel.ForeColor = Color.FromArgb(245, 248, 255);
        railTitleLabel.Location = new Point(24, 22);
        railTitleLabel.Name = "railTitleLabel";
        railTitleLabel.Size = new Size(140, 28);
        railTitleLabel.TabIndex = 0;
        railTitleLabel.Text = "Access Keys";
        // 
        // textPanel
        // 
        textPanel.Controls.Add(clearAllButton);
        textPanel.Controls.Add(copyTextButton);
        textPanel.Controls.Add(decryptTextButton);
        textPanel.Controls.Add(encryptTextButton);
        textPanel.Controls.Add(outputTextBox);
        textPanel.Controls.Add(outputLabel);
        textPanel.Controls.Add(inputTextBox);
        textPanel.Controls.Add(inputLabel);
        textPanel.Controls.Add(textPanelDescription);
        textPanel.Location = new Point(326, 180);
        textPanel.Name = "textPanel";
        textPanel.Size = new Size(446, 526);
        textPanel.TabIndex = 2;
        // 
        // clearAllButton
        // 
        clearAllButton.ForeColor = Color.FromArgb(245, 248, 255);
        clearAllButton.Location = new Point(228, 457);
        clearAllButton.Name = "clearAllButton";
        clearAllButton.Size = new Size(180, 38);
        clearAllButton.TabIndex = 8;
        clearAllButton.Text = "Clear all";
        clearAllButton.UseVisualStyleBackColor = false;
        clearAllButton.Click += ClearAllButton_Click;
        // 
        // copyTextButton
        // 
        copyTextButton.ForeColor = Color.FromArgb(245, 248, 255);
        copyTextButton.Location = new Point(30, 457);
        copyTextButton.Name = "copyTextButton";
        copyTextButton.Size = new Size(180, 38);
        copyTextButton.TabIndex = 7;
        copyTextButton.Text = "Copy output";
        copyTextButton.UseVisualStyleBackColor = false;
        copyTextButton.Click += CopyTextButton_Click;
        // 
        // decryptTextButton
        // 
        decryptTextButton.ForeColor = Color.FromArgb(245, 248, 255);
        decryptTextButton.Location = new Point(228, 70);
        decryptTextButton.Name = "decryptTextButton";
        decryptTextButton.Size = new Size(180, 38);
        decryptTextButton.TabIndex = 6;
        decryptTextButton.Text = "Decrypt";
        decryptTextButton.UseVisualStyleBackColor = false;
        decryptTextButton.Click += DecryptTextButton_Click;
        // 
        // encryptTextButton
        // 
        encryptTextButton.ForeColor = Color.FromArgb(245, 248, 255);
        encryptTextButton.Location = new Point(30, 70);
        encryptTextButton.Name = "encryptTextButton";
        encryptTextButton.Size = new Size(180, 38);
        encryptTextButton.TabIndex = 5;
        encryptTextButton.Text = "Encrypt";
        encryptTextButton.UseVisualStyleBackColor = false;
        encryptTextButton.Click += EncryptTextButton_Click;
        // 
        // outputTextBox
        // 
        outputTextBox.Location = new Point(30, 302);
        outputTextBox.Name = "outputTextBox";
        outputTextBox.Size = new Size(386, 132);
        outputTextBox.TabIndex = 4;
        // 
        // outputLabel
        // 
        outputLabel.AutoSize = true;
        outputLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        outputLabel.ForeColor = Color.FromArgb(245, 248, 255);
        outputLabel.Location = new Point(30, 279);
        outputLabel.Name = "outputLabel";
        outputLabel.Size = new Size(93, 19);
        outputLabel.TabIndex = 3;
        outputLabel.Text = "Output";
        // 
        // inputTextBox
        // 
        inputTextBox.Location = new Point(30, 144);
        inputTextBox.Name = "inputTextBox";
        inputTextBox.Size = new Size(386, 112);
        inputTextBox.TabIndex = 2;
        // 
        // inputLabel
        // 
        inputLabel.AutoSize = true;
        inputLabel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        inputLabel.ForeColor = Color.FromArgb(245, 248, 255);
        inputLabel.Location = new Point(30, 121);
        inputLabel.Name = "inputLabel";
        inputLabel.Size = new Size(84, 19);
        inputLabel.TabIndex = 1;
        inputLabel.Text = "Input";
        // 
        // textPanelDescription
        // 
        textPanelDescription.AutoSize = true;
        textPanelDescription.Font = new Font("Bahnschrift SemiBold", 17F, FontStyle.Bold);
        textPanelDescription.ForeColor = Color.FromArgb(245, 248, 255);
        textPanelDescription.Location = new Point(30, 22);
        textPanelDescription.Name = "textPanelDescription";
        textPanelDescription.Size = new Size(194, 28);
        textPanelDescription.TabIndex = 0;
        textPanelDescription.Text = "Text Console";
        // 
        // imagePanel
        // 
        imagePanel.Controls.Add(imageDropHintLabel);
        imagePanel.Controls.Add(decryptImageButton);
        imagePanel.Controls.Add(selectEncryptedImageButton);
        imagePanel.Controls.Add(encryptImageButton);
        imagePanel.Controls.Add(selectImageButton);
        imagePanel.Controls.Add(encryptedImagePathValueLabel);
        imagePanel.Controls.Add(encryptedImagePathLabel);
        imagePanel.Controls.Add(imagePathValueLabel);
        imagePanel.Controls.Add(imagePathLabel);
        imagePanel.Controls.Add(previewPanel);
        imagePanel.Controls.Add(imagePanelDescription);
        imagePanel.Controls.Add(imagePanelTitle);
        imagePanel.Location = new Point(792, 180);
        imagePanel.Name = "imagePanel";
        imagePanel.Size = new Size(432, 526);
        imagePanel.TabIndex = 3;
        // 
        // imageDropHintLabel
        // 
        imageDropHintLabel.Font = new Font("Segoe UI", 9F);
        imageDropHintLabel.Location = new Point(30, 464);
        imageDropHintLabel.Name = "imageDropHintLabel";
        imageDropHintLabel.Size = new Size(360, 32);
        imageDropHintLabel.TabIndex = 11;
        imageDropHintLabel.Text = "Drop an image or a `.mseimg` file here.";
        // 
        // decryptImageButton
        // 
        decryptImageButton.ForeColor = Color.FromArgb(245, 248, 255);
        decryptImageButton.Location = new Point(222, 409);
        decryptImageButton.Name = "decryptImageButton";
        decryptImageButton.Size = new Size(180, 38);
        decryptImageButton.TabIndex = 10;
        decryptImageButton.Text = "Decrypt image";
        decryptImageButton.UseVisualStyleBackColor = false;
        decryptImageButton.Click += DecryptImageButton_Click;
        // 
        // selectEncryptedImageButton
        // 
        selectEncryptedImageButton.ForeColor = Color.FromArgb(245, 248, 255);
        selectEncryptedImageButton.Location = new Point(30, 409);
        selectEncryptedImageButton.Name = "selectEncryptedImageButton";
        selectEncryptedImageButton.Size = new Size(170, 38);
        selectEncryptedImageButton.TabIndex = 9;
        selectEncryptedImageButton.Text = "Open encrypted file";
        selectEncryptedImageButton.UseVisualStyleBackColor = false;
        selectEncryptedImageButton.Click += SelectEncryptedImageButton_Click;
        // 
        // encryptImageButton
        // 
        encryptImageButton.ForeColor = Color.FromArgb(245, 248, 255);
        encryptImageButton.Location = new Point(222, 358);
        encryptImageButton.Name = "encryptImageButton";
        encryptImageButton.Size = new Size(180, 38);
        encryptImageButton.TabIndex = 8;
        encryptImageButton.Text = "Encrypt image";
        encryptImageButton.UseVisualStyleBackColor = false;
        encryptImageButton.Click += EncryptImageButton_Click;
        // 
        // selectImageButton
        // 
        selectImageButton.ForeColor = Color.FromArgb(245, 248, 255);
        selectImageButton.Location = new Point(30, 358);
        selectImageButton.Name = "selectImageButton";
        selectImageButton.Size = new Size(170, 38);
        selectImageButton.TabIndex = 7;
        selectImageButton.Text = "Open image";
        selectImageButton.UseVisualStyleBackColor = false;
        selectImageButton.Click += SelectImageButton_Click;
        // 
        // encryptedImagePathValueLabel
        // 
        encryptedImagePathValueLabel.AutoEllipsis = true;
        encryptedImagePathValueLabel.Font = new Font("Segoe UI", 9F);
        encryptedImagePathValueLabel.Location = new Point(30, 330);
        encryptedImagePathValueLabel.Name = "encryptedImagePathValueLabel";
        encryptedImagePathValueLabel.Size = new Size(360, 16);
        encryptedImagePathValueLabel.TabIndex = 6;
        encryptedImagePathValueLabel.Text = "No encrypted file selected";
        // 
        // encryptedImagePathLabel
        // 
        encryptedImagePathLabel.AutoSize = true;
        encryptedImagePathLabel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        encryptedImagePathLabel.ForeColor = Color.FromArgb(245, 248, 255);
        encryptedImagePathLabel.Location = new Point(30, 308);
        encryptedImagePathLabel.Name = "encryptedImagePathLabel";
        encryptedImagePathLabel.Size = new Size(140, 17);
        encryptedImagePathLabel.TabIndex = 5;
        encryptedImagePathLabel.Text = "Encrypted file";
        // 
        // imagePathValueLabel
        // 
        imagePathValueLabel.AutoEllipsis = true;
        imagePathValueLabel.Font = new Font("Segoe UI", 9F);
        imagePathValueLabel.Location = new Point(30, 278);
        imagePathValueLabel.Name = "imagePathValueLabel";
        imagePathValueLabel.Size = new Size(360, 16);
        imagePathValueLabel.TabIndex = 4;
        imagePathValueLabel.Text = "No image selected";
        // 
        // imagePathLabel
        // 
        imagePathLabel.AutoSize = true;
        imagePathLabel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        imagePathLabel.ForeColor = Color.FromArgb(245, 248, 255);
        imagePathLabel.Location = new Point(30, 256);
        imagePathLabel.Name = "imagePathLabel";
        imagePathLabel.Size = new Size(106, 17);
        imagePathLabel.TabIndex = 3;
        imagePathLabel.Text = "Selected image";
        // 
        // previewPanel
        // 
        previewPanel.Controls.Add(imagePreviewBox);
        previewPanel.Location = new Point(30, 103);
        previewPanel.Name = "previewPanel";
        previewPanel.Size = new Size(372, 132);
        previewPanel.TabIndex = 2;
        // 
        // imagePreviewBox
        // 
        imagePreviewBox.Location = new Point(13, 13);
        imagePreviewBox.Name = "imagePreviewBox";
        imagePreviewBox.Size = new Size(346, 106);
        imagePreviewBox.SizeMode = PictureBoxSizeMode.Zoom;
        imagePreviewBox.TabIndex = 0;
        imagePreviewBox.TabStop = false;
        // 
        // imagePanelDescription
        // 
        imagePanelDescription.AutoSize = true;
        imagePanelDescription.Font = new Font("Segoe UI", 9.5F);
        imagePanelDescription.ForeColor = Color.FromArgb(148, 163, 184);
        imagePanelDescription.Location = new Point(30, 58);
        imagePanelDescription.Name = "imagePanelDescription";
        imagePanelDescription.Size = new Size(253, 17);
        imagePanelDescription.TabIndex = 1;
        imagePanelDescription.Text = "Preview and encryption tools for image files.";
        // 
        // imagePanelTitle
        // 
        imagePanelTitle.AutoSize = true;
        imagePanelTitle.Font = new Font("Bahnschrift SemiBold", 17F, FontStyle.Bold);
        imagePanelTitle.ForeColor = Color.FromArgb(245, 248, 255);
        imagePanelTitle.Location = new Point(30, 24);
        imagePanelTitle.Name = "imagePanelTitle";
        imagePanelTitle.Size = new Size(136, 28);
        imagePanelTitle.TabIndex = 0;
        imagePanelTitle.Text = "Image Console";
        // 
        // statusPanel
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(3, 0, 20);
        ClientSize = new Size(1296, 734);
        Controls.Add(imagePanel);
        Controls.Add(textPanel);
        Controls.Add(keyRailPanel);
        Controls.Add(heroPanel);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "MagSec Encrypter";
        heroPanel.ResumeLayout(false);
        heroPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)brandPictureBox).EndInit();
        keyRailPanel.ResumeLayout(false);
        keyRailPanel.PerformLayout();
        textPanel.ResumeLayout(false);
        textPanel.PerformLayout();
        imagePanel.ResumeLayout(false);
        imagePanel.PerformLayout();
        previewPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)imagePreviewBox).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private GlassPanel heroPanel;
    private Label heroMetaLabel;
    private Label subtitleLabel;
    private Label titleLabel;
    private Label heroKickerLabel;
    private PictureBox brandPictureBox;
    private GlassPanel keyRailPanel;
    private Label sideInfoLabel;
    private TextBox key4TextBox;
    private Label key4Label;
    private TextBox key3TextBox;
    private Label key3Label;
    private TextBox key2TextBox;
    private Label key2Label;
    private TextBox key1TextBox;
    private Label key1Label;
    private Label railDescriptionLabel;
    private Label railTitleLabel;
    private GlassPanel textPanel;
    private NeonButton clearAllButton;
    private NeonButton copyTextButton;
    private NeonButton decryptTextButton;
    private NeonButton encryptTextButton;
    private StyledEditor outputTextBox;
    private Label outputLabel;
    private StyledEditor inputTextBox;
    private Label inputLabel;
    private Label textPanelDescription;
    private GlassPanel imagePanel;
    private Label imageDropHintLabel;
    private NeonButton decryptImageButton;
    private NeonButton selectEncryptedImageButton;
    private NeonButton encryptImageButton;
    private NeonButton selectImageButton;
    private Label encryptedImagePathValueLabel;
    private Label encryptedImagePathLabel;
    private Label imagePathValueLabel;
    private Label imagePathLabel;
    private GlassPanel previewPanel;
    private PictureBox imagePreviewBox;
    private Label imagePanelDescription;
    private Label imagePanelTitle;
}
