using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}

internal static class UiTheme
{
    public static readonly Color Background = Color.FromArgb(15, 18, 24);
    public static readonly Color PanelBack = Color.FromArgb(24, 29, 38);
    public static readonly Color PanelAlt = Color.FromArgb(31, 38, 49);
    public static readonly Color Input = Color.FromArgb(12, 15, 20);
    public static readonly Color Border = Color.FromArgb(78, 90, 110);
    public static readonly Color Text = Color.FromArgb(238, 243, 248);
    public static readonly Color MutedText = Color.FromArgb(174, 184, 199);
    public static readonly Color Accent = Color.FromArgb(64, 126, 219);
    public static readonly Color AccentText = Color.FromArgb(141, 198, 255);
    public static readonly Color Selection = Color.FromArgb(22, 92, 170);
    public static readonly Color SelectionText = Color.White;
    public static readonly Color Success = Color.FromArgb(105, 220, 145);
    public static readonly Color Error = Color.FromArgb(255, 112, 112);
    public static readonly Color FileBack = Color.FromArgb(67, 42, 12);
    public static readonly Color FileFore = Color.FromArgb(255, 196, 98);
    public static readonly Color GameNameBack = Color.FromArgb(62, 57, 10);
    public static readonly Color GameNameFore = Color.FromArgb(255, 235, 99);
    public static readonly Color EvolutionBack = Color.FromArgb(12, 61, 48);
    public static readonly Color EvolutionFore = Color.FromArgb(126, 247, 205);
    public static readonly Color HighlightBack = Color.FromArgb(29, 60, 96);

    public static void Apply(Control root)
    {
        root.BackColor = root is TextBoxBase or ComboBox or ListBox ? Input : Background;
        root.ForeColor = Text;

        switch (root)
        {
            case Form:
            case TabControl:
                root.BackColor = Background;
                break;
            case TableLayoutPanel:
            case FlowLayoutPanel:
            case TabPage:
            case GroupBox:
            case Panel:
                root.BackColor = PanelBack;
                break;
            case TextBoxBase textBox:
                textBox.BackColor = Input;
                textBox.ForeColor = Text;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                break;
            case ListBox listBox:
                listBox.BackColor = Input;
                listBox.ForeColor = Text;
                break;
            case Button button:
                button.BackColor = Accent;
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Border;
                button.FlatAppearance.BorderSize = 1;
                break;
            case Label label:
                label.BackColor = Color.Transparent;
                label.ForeColor = Text;
                break;
            case PictureBox pictureBox:
                pictureBox.BackColor = Input;
                break;
        }

        foreach (Control child in root.Controls)
            Apply(child);
    }

    public static void ApplyGrid(DataGridView grid)
    {
        grid.BackgroundColor = Input;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.GridColor = Border;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = PanelAlt;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = PanelAlt;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Text;
        grid.DefaultCellStyle.BackColor = Input;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.SelectionBackColor = Selection;
        grid.DefaultCellStyle.SelectionForeColor = SelectionText;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(17, 21, 28);
        grid.AlternatingRowsDefaultCellStyle.ForeColor = Text;
        grid.RowHeadersDefaultCellStyle.BackColor = PanelAlt;
        grid.RowHeadersDefaultCellStyle.ForeColor = Text;
    }

    public static void ApplyTabs(TabControl tabs)
    {
        tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabs.BackColor = Background;
        tabs.ForeColor = Text;
        tabs.DrawItem += (_, e) =>
        {
            var selected = e.Index == tabs.SelectedIndex;
            using var back = new SolidBrush(selected ? PanelAlt : Background);
            using var fore = new SolidBrush(selected ? Color.White : MutedText);
            e.Graphics.FillRectangle(back, e.Bounds);
            var text = tabs.TabPages[e.Index].Text;
            TextRenderer.DrawText(e.Graphics, text, tabs.Font, e.Bounds, selected ? Color.White : MutedText, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        };
    }
}

internal static class LocalizedText
{
    private static readonly Dictionary<string, Dictionary<string, string>> Tables = new()
    {
        ["EN"] = new Dictionary<string, string>
        {
            ["Monster Card EXE Editor"] = "Monster Card EXE Editor",
            ["비밀번호"] = "Password",
            ["카드 폴더 열기"] = "Open Card Folder",
            ["기본폴더 사용"] = "Use Default Folder",
            ["ES3Defaults.asset"] = "ES3Defaults.asset",
            ["목록 새로고침"] = "Refresh List",
            ["카드 저장"] = "Save Card",
            ["카드 제거"] = "Delete Card",
            ["카드 파일명[실제 디렉토리]"] = "Card File [Actual Directory]",
            ["카드 인게임 출력명"] = "In-game Card Name",
            ["카드 파일명"] = "Card Files",
            ["기본 카드 폴더"] = "Default Card Folder",
            ["지정 카드 폴더"] = "Custom Card Folder",
            ["Tag 검색"] = "Tag Search",
            ["Tag 선택"] = "Select Tag",
            ["파일명: -"] = "File: -",
            ["파일명"] = "File Name",
            ["카드 이름"] = "Card Name",
            ["특수 링크"] = "Special Link",
            ["주요 텍스트"] = "Main Text",
            ["실제 텍스트 필드"] = "Text Fields",
            ["플레이버 텍스트"] = "Flavor Text",
            ["플레이버/TAG"] = "Flavor/TAG",
            ["카드 플레이버 텍스트"] = "Card Flavor Text",
            ["Tag"] = "Tag",
            ["Tag 1"] = "Tag 1",
            ["Tag 2"] = "Tag 2",
            ["Tag 3"] = "Tag 3",
            ["플레이버 자동 출력"] = "Auto Flavor Text",
            ["수치"] = "Value",
            ["종족명"] = "Race Name",
            ["속성명"] = "Element Name",
            ["여기에 설명을 입력하세요"] = "Your Description Here",
            ["제작자명"] = "Creator Name",
            ["플레이버 텍스트 자동 출력 완료"] = "Auto flavor text generated.",
            ["확대/축소"] = "Scale",
            ["카드 프레임"] = "Card Frame",
            ["이미지/BGM bytes"] = "Image/BGM Bytes",
            ["고급 JSON"] = "Advanced JSON",
            ["텍스트 필드 적용"] = "Apply Text Fields",
            ["JSON 적용"] = "Apply JSON",
            ["필드명"] = "Field",
            ["값"] = "Value",
            ["종류"] = "Type",
            ["대상 파일명"] = "Target File",
            ["대상 인게임 이름"] = "Target In-game Name",
            ["대상 UUID"] = "Target UUID",
            ["갱신일"] = "Updated",
            ["대기 중"] = "Idle",
            ["저장 시 복호화/압축해제/JSON 파싱 검증 후 .card를 생성합니다.\n본 에디터를 사용하여 오는 불이익에 대해선 제작자 KaZaRI는 책임을 지지 않습니다."] =
                "On save, the .card is verified by decrypt/decompress/JSON parse.\nCreator KaZaRI is not responsible for any disadvantage caused by using this editor.",
            ["권장 이미지 크기: 카드를 선택하면 표시됩니다. 크기가 다르면 자동 리사이즈합니다. (PNG)"] =
                "Recommended image size appears after selecting a card. Different sizes are resized automatically. (PNG)",
            ["권장 이미지 크기: PNG 이미지를 사용하세요. 크기가 다르면 자동 리사이즈합니다."] =
                "Recommended image size: use PNG. Different sizes are resized automatically.",
            ["권장 이미지 크기"] = "Recommended image size",
            ["카드 검은 영역 원본 PNG"] = "Original PNG for card black image area",
            ["현재 레이어 크기"] = "Current layer size",
            ["크기가 다르면 자동 리사이즈"] = "different sizes are resized",
            ["없음"] = "None",
            ["교체"] = "Replace",
            ["추출"] = "Export",
            ["현재 파일명: none"] = "Current file: none",
            ["필드 없음"] = "Field missing",
            ["BGM 바이트\n교체/추출 가능"] = "BGM bytes\nReplace/export available",
            ["미리보기 없음"] = "No preview",
            ["목록 필터 ON"] = "List filter ON",
            ["레어도 순"] = "Rarity order",
            ["ㄱ~ㄴ / A~Z / あ~順"] = "Name order",
            ["Tag 순"] = "Tag order",
            ["image1/2/3 공통 이미지 조절"] = "Shared image1/2/3 controls",
            ["압축률"] = "Compression",
            ["압축 없음"] = "No compression",
            ["파일 이미지 용량을 위해 x512를 추천"] = "x512 is recommended to reduce image file size",
            ["630x880 (9:13)의 이미지일 경우 70%로 수정을 권장"] = "For 630x880 (9:13) images, 70% scale is recommended",
            ["초기화"] = "Reset",
            ["색수차"] = "Chromatic",
            ["휴"] = "Hue",
            ["새츄레이션"] = "Saturation",
            ["후처리1"] = "Post 1",
            ["후처리2"] = "Post 2",
            ["후처리3"] = "Post 3",
            ["강도"] = "Strength",
            ["효과 없음"] = "No effect",
            ["레이어 미리보기 없음"] = "No layer preview",
            ["레이어 1+2+3 미리보기"] = "Layer 1+2+3 Preview",
            ["Lens Flare"] = "Lens Flare",
            ["Bloom"] = "Bloom",
            ["Depth of Field"] = "Depth of Field",
            ["Blur"] = "Blur",
            ["Film Grain"] = "Film Grain",
            ["Noise"] = "Noise",
            ["Vignette"] = "Vignette",
            ["Contrast"] = "Contrast",
            ["Monochrome"] = "Monochrome",
            ["CRT Scanlines"] = "CRT Scanlines",
            ["Pixelation"] = "Pixelation",
            ["Halftone"] = "Halftone",
            ["Glitch"] = "Glitch",
            ["RGB Split"] = "RGB Split",
            ["TV Static"] = "TV Static",
            ["Interlace"] = "Interlace",
        },
        ["JP"] = new Dictionary<string, string>
        {
            ["Monster Card EXE Editor"] = "Monster Card EXE Editor",
            ["비밀번호"] = "パスワード",
            ["카드 폴더 열기"] = "カードフォルダーを開く",
            ["기본폴더 사용"] = "既定フォルダーを使用",
            ["ES3Defaults.asset"] = "ES3Defaults.asset",
            ["목록 새로고침"] = "リスト更新",
            ["카드 저장"] = "カード保存",
            ["카드 제거"] = "カード削除",
            ["카드 파일명[실제 디렉토리]"] = "カードファイル名[実ディレクトリ]",
            ["카드 인게임 출력명"] = "ゲーム内表示名",
            ["카드 파일명"] = "カードファイル名",
            ["기본 카드 폴더"] = "既定カードフォルダー",
            ["지정 카드 폴더"] = "指定カードフォルダー",
            ["Tag 검색"] = "Tag検索",
            ["Tag 선택"] = "Tag選択",
            ["파일명: -"] = "ファイル名: -",
            ["파일명"] = "ファイル名",
            ["카드 이름"] = "カード名",
            ["특수 링크"] = "特殊リンク",
            ["주요 텍스트"] = "主要テキスト",
            ["실제 텍스트 필드"] = "実テキスト項目",
            ["플레이버 텍스트"] = "フレーバーテキスト",
            ["플레이버/TAG"] = "フレーバー/TAG",
            ["카드 플레이버 텍스트"] = "カードのフレーバーテキスト",
            ["Tag"] = "Tag",
            ["Tag 1"] = "Tag 1",
            ["Tag 2"] = "Tag 2",
            ["Tag 3"] = "Tag 3",
            ["플레이버 자동 출력"] = "フレーバー自動出力",
            ["수치"] = "数値",
            ["종족명"] = "種族名",
            ["속성명"] = "属性名",
            ["여기에 설명을 입력하세요"] = "説明を入力してください",
            ["제작자명"] = "作者名",
            ["플레이버 텍스트 자동 출력 완료"] = "フレーバーテキストを自動出力しました。",
            ["확대/축소"] = "拡大/縮小",
            ["카드 프레임"] = "カードフレーム",
            ["이미지/BGM bytes"] = "画像/BGM bytes",
            ["고급 JSON"] = "高度なJSON",
            ["텍스트 필드 적용"] = "テキスト適用",
            ["JSON 적용"] = "JSON適用",
            ["필드명"] = "項目名",
            ["값"] = "値",
            ["종류"] = "種類",
            ["대상 파일명"] = "対象ファイル名",
            ["대상 인게임 이름"] = "対象ゲーム内名",
            ["대상 UUID"] = "対象UUID",
            ["갱신일"] = "更新日",
            ["대기 중"] = "待機中",
            ["저장 시 복호화/압축해제/JSON 파싱 검증 후 .card를 생성합니다.\n본 에디터를 사용하여 오는 불이익에 대해선 제작자 KaZaRI는 책임을 지지 않습니다."] =
                "保存時に復号/展開/JSON解析で検証してから .card を生成します。\n本エディターの使用による不利益について、製作者KaZaRIは責任を負いません。",
            ["권장 이미지 크기: 카드를 선택하면 표시됩니다. 크기가 다르면 자동 리사이즈합니다. (PNG)"] =
                "推奨画像サイズはカード選択後に表示されます。サイズが違う場合は自動リサイズします。(PNG)",
            ["권장 이미지 크기: PNG 이미지를 사용하세요. 크기가 다르면 자동 리사이즈합니다."] =
                "推奨画像サイズ: PNGを使用してください。サイズが違う場合は自動リサイズします。",
            ["권장 이미지 크기"] = "推奨画像サイズ",
            ["카드 검은 영역 원본 PNG"] = "カード黒画像領域の元PNG",
            ["현재 레이어 크기"] = "現在のレイヤーサイズ",
            ["크기가 다르면 자동 리사이즈"] = "サイズ違いは自動リサイズ",
            ["없음"] = "なし",
            ["교체"] = "置換",
            ["추출"] = "抽出",
            ["현재 파일명: none"] = "現在のファイル名: なし",
            ["필드 없음"] = "項目なし",
            ["BGM 바이트\n교체/추출 가능"] = "BGM bytes\n置換/抽出可能",
            ["미리보기 없음"] = "プレビューなし",
            ["목록 필터 ON"] = "リストフィルターON",
            ["레어도 순"] = "レア度順",
            ["ㄱ~ㄴ / A~Z / あ~順"] = "名前順",
            ["Tag 순"] = "Tag順",
            ["image1/2/3 공통 이미지 조절"] = "image1/2/3 共通画像調整",
            ["압축률"] = "圧縮率",
            ["압축 없음"] = "圧縮なし",
            ["파일 이미지 용량을 위해 x512를 추천"] = "画像容量を抑えるため x512 推奨",
            ["630x880 (9:13)의 이미지일 경우 70%로 수정을 권장"] = "630x880 (9:13) の画像は70%調整を推奨",
            ["초기화"] = "初期化",
            ["색수차"] = "色収差",
            ["휴"] = "色相",
            ["새츄레이션"] = "彩度",
            ["후처리1"] = "後処理1",
            ["후처리2"] = "後処理2",
            ["후처리3"] = "後処理3",
            ["강도"] = "強度",
            ["효과 없음"] = "効果なし",
            ["레이어 미리보기 없음"] = "レイヤープレビューなし",
            ["레이어 1+2+3 미리보기"] = "レイヤー1+2+3プレビュー",
            ["Lens Flare"] = "レンズフレア",
            ["Bloom"] = "ブルーム",
            ["Depth of Field"] = "被写界深度",
            ["Blur"] = "ブラー",
            ["Film Grain"] = "フィルム粒子",
            ["Noise"] = "ノイズ",
            ["Vignette"] = "ビネット",
            ["Contrast"] = "コントラスト",
            ["Monochrome"] = "モノクロ",
            ["CRT Scanlines"] = "CRT走査線",
            ["Pixelation"] = "ピクセル化",
            ["Halftone"] = "ハーフトーン",
            ["Glitch"] = "グリッチ",
            ["RGB Split"] = "RGB分離",
            ["TV Static"] = "TV砂嵐",
            ["Interlace"] = "インターレース",
        },
        ["KR"] = new Dictionary<string, string>(),
    };

    public static string Translate(string language, string key)
    {
        if (language != "KR" && Tables.TryGetValue(language, out var table) && table.TryGetValue(key, out var translated))
            return translated;
        return key;
    }
}

internal sealed class MainForm : Form
{
    private readonly TextBox _passwordBox = new() { Text = "twoweeks", Dock = DockStyle.Fill };
    private readonly TextBox _folderBox = new() { ReadOnly = true, Dock = DockStyle.Fill };
    private readonly TextBox _defaultsBox = new() { ReadOnly = true, Dock = DockStyle.Fill };
    private readonly Button _chooseFolderButton = new() { Text = "카드 폴더 열기", Dock = DockStyle.Fill };
    private readonly Button _useDefaultGameFolderButton = new() { Text = "기본폴더 사용", Dock = DockStyle.Fill };
    private readonly Button _chooseDefaultsButton = new() { Text = "ES3Defaults.asset", Dock = DockStyle.Fill };
    private readonly Button _reloadButton = new() { Text = "목록 새로고침", Dock = DockStyle.Fill };
    private readonly ListBox _defaultCardList = new();
    private readonly ListBox _customCardList = new();
    private readonly GroupBox _customCardsGroup = new() { Text = "지정 카드 폴더", Dock = DockStyle.Fill, Padding = new Padding(4) };
    private readonly Button _moveToDefaultButton = new() { Text = "▲", Width = 48 };
    private readonly Button _moveToCustomButton = new() { Text = "▼", Width = 48 };
    private readonly Button _deleteCardButton = new() { Text = "카드 제거", Width = 92 };
    private readonly CheckBox _cardFilterEnabledBox = new() { Text = "목록 필터 ON", AutoSize = true };
    private readonly ComboBox _cardFilterModeBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 176 };
    private readonly ComboBox _tagFilterBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
    private readonly Label _fileNameLabel = new() { AutoSize = false, Dock = DockStyle.Fill, Text = "파일명: -", TextAlign = ContentAlignment.MiddleLeft };
    private readonly TextBox _currentFileDisplayBox = new() { ReadOnly = true, Dock = DockStyle.Fill };
    private readonly TextBox _currentGameNameDisplayBox = new() { ReadOnly = true, Dock = DockStyle.Fill };
    private readonly TextBox _fileNameBox = new() { Dock = DockStyle.Fill };
    private readonly TextBox _cardNameBox = new() { Dock = DockStyle.Fill };
    private readonly TextBox _evolutionSummaryBox = new() { Dock = DockStyle.Fill, ReadOnly = true, ScrollBars = ScrollBars.Horizontal, WordWrap = false };
    private readonly TextBox _flavorTextBox = new() { Multiline = true, ScrollBars = ScrollBars.Vertical, MaxLength = FlavorTextLimit };
    private readonly Label _flavorCountLabel = new() { AutoSize = false, Width = 74, Height = 28, TextAlign = ContentAlignment.MiddleRight };
    private readonly TextBox _tagTextBox1 = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tagTextBox2 = new() { Dock = DockStyle.Fill };
    private readonly TextBox _tagTextBox3 = new() { Dock = DockStyle.Fill };
    private readonly Button _autoFlavorButton = new() { Text = "플레이버 자동 출력", Width = 140 };
    private readonly DataGridView _textGrid = new();
    private readonly DataGridView _evolutionGrid = new();
    private readonly TextBox _jsonBox = new() { Multiline = true, ScrollBars = ScrollBars.Both, WordWrap = false, Font = new Font("Consolas", 9), ReadOnly = true };
    private readonly TabControl _tabs = new();
    private TabPage? _imagePage;
    private readonly Label _imageSizeHintLabel = new() { AutoSize = false, Dock = DockStyle.Fill, Height = 30, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft };
    private readonly ComboBox _globalDotStyleBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 104 };
    private readonly TrackBar _globalChromaticTrack = new() { Minimum = 0, Maximum = 20, TickFrequency = 5, SmallChange = 1, LargeChange = 4, Width = 130, Height = 30 };
    private readonly TrackBar _globalHueTrack = new() { Minimum = -180, Maximum = 180, TickFrequency = 60, SmallChange = 5, LargeChange = 30, Width = 150, Height = 30 };
    private readonly TrackBar _globalSaturationTrack = new() { Minimum = -100, Maximum = 100, TickFrequency = 50, SmallChange = 5, LargeChange = 20, Width = 150, Height = 30 };
    private readonly Label _globalScaleLabel = new() { AutoSize = false, Width = 88, TextAlign = ContentAlignment.MiddleLeft };
    private readonly TrackBar _globalScaleTrack = new() { Minimum = 10, Maximum = 300, Value = 100, TickFrequency = 25, SmallChange = 5, LargeChange = 25, Width = 220, Height = 30 };
    private readonly NumericUpDown _globalScaleNumber = new() { Minimum = 10, Maximum = 300, Value = 100, Width = 62 };
    private readonly Label _globalCompressionHintLabel = new() { AutoSize = true, Height = 24, TextAlign = ContentAlignment.MiddleLeft };
    private readonly Label _globalScaleHintLabel = new() { AutoSize = true, Height = 24, TextAlign = ContentAlignment.MiddleLeft };
    private readonly Label _globalChromaticLabel = new() { AutoSize = false, Width = 74, TextAlign = ContentAlignment.MiddleLeft };
    private readonly Label _globalHueLabel = new() { AutoSize = false, Width = 62, TextAlign = ContentAlignment.MiddleLeft };
    private readonly Label _globalSaturationLabel = new() { AutoSize = false, Width = 94, TextAlign = ContentAlignment.MiddleLeft };
    private readonly Button _globalResetImageEditsButton = new() { Text = "초기화", Width = 76 };
    private readonly Button _applyTextButton = new() { Text = "텍스트 필드 적용" };
    private readonly Button _applyJsonButton = new() { Text = "JSON 적용" };
    private readonly Button _saveButton = new() { Text = "카드 저장", Dock = DockStyle.Fill };
    private readonly Button _langEnButton = new() { Text = "EN", Width = 48 };
    private readonly Button _langJpButton = new() { Text = "JP", Width = 48 };
    private readonly Button _langKrButton = new() { Text = "KR", Width = 48 };
    private readonly Label _actionNoteLabel = new() { AutoSize = true, Padding = new Padding(12, 10, 0, 0) };
    private readonly Label _statusLabel = new() { AutoSize = false, Height = 28, Text = "대기 중" };
    private readonly ImagePanel[] _imagePanels;
    private ImagePreviewForm? _imagePreviewForm;

    private readonly List<CardListItem> _defaultCards = new();
    private readonly List<CardListItem> _customCards = new();
    private string? _defaultsPath;
    private CardSettings _settings = CardSettings.Default;
    private LoadedCard? _current;
    private bool _loadingGrid;
    private bool _suppressGlobalImageEdits;
    private bool _syncingGlobalScale;
    private bool _updatingTagFilter;
    private string _language = "KR";
    private static readonly string[] ImageByteFields = ["image1Bytes", "image2Bytes", "image3Bytes"];
    private const string CardTagField = "tags";
    private const int FlavorTextLimit = 300;
    private const int CardTextureWidth = 630;
    private const int CardTextureHeight = 880;

    public MainForm()
    {
        Text = "Monster Card EXE Editor";
        Width = 1500;
        Height = 960;
        MinimumSize = new Size(1280, 780);
        StartPosition = FormStartPosition.CenterScreen;

        _imagePanels = new[]
        {
            new ImagePanel("image1Bytes"),
            new ImagePanel("image2Bytes"),
            new ImagePanel("image3Bytes"),
            new ImagePanel("bgm"),
        };

        BuildUi();
        WireEvents();
        SetEditorEnabled(false);
        UpdateCurrentDisplay();
        ReloadCards();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        CloseImagePreviewPopup();
        base.OnFormClosed(e);
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Padding = new Padding(10),
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 138));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        Controls.Add(root);

        var top = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 3 };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        top.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        top.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        top.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.SetColumnSpan(top, 2);
        root.Controls.Add(top, 0, 0);

        top.Controls.Add(MakeLabel("비밀번호"), 0, 0);
        top.Controls.Add(_passwordBox, 1, 0);
        top.Controls.Add(_chooseDefaultsButton, 2, 0);
        top.Controls.Add(_defaultsBox, 3, 0);
        top.Controls.Add(_reloadButton, 4, 0);
        top.Controls.Add(_saveButton, 5, 0);
        top.Controls.Add(_chooseFolderButton, 0, 1);
        top.SetColumnSpan(_folderBox, 2);
        top.Controls.Add(_folderBox, 1, 1);
        top.Controls.Add(_useDefaultGameFolderButton, 3, 1);
        top.SetColumnSpan(_useDefaultGameFolderButton, 2);

        var currentInfo = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1 };
        currentInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 210));
        currentInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        currentInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        currentInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        currentInfo.Controls.Add(MakeLabel("카드 파일명[실제 디렉토리]"), 0, 0);
        currentInfo.Controls.Add(_currentFileDisplayBox, 1, 0);
        currentInfo.Controls.Add(MakeLabel("카드 인게임 출력명"), 2, 0);
        currentInfo.Controls.Add(_currentGameNameDisplayBox, 3, 0);
        top.Controls.Add(currentInfo, 0, 2);
        top.SetColumnSpan(currentInfo, 6);

        ResetComboItems(_cardFilterModeBox, 0, "레어도 순", "ㄱ~ㄴ / A~Z / あ~順", "Tag 순");

        var left = new GroupBox { Text = "카드 파일명", Dock = DockStyle.Fill, Padding = new Padding(8) };
        var leftLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var filterRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        filterRow.Controls.Add(_cardFilterEnabledBox);
        filterRow.Controls.Add(_cardFilterModeBox);
        leftLayout.Controls.Add(filterRow, 0, 0);
        var tagSearchRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        tagSearchRow.Controls.Add(new Label { Text = "Tag 선택", AutoSize = false, Width = 72, Height = 24, TextAlign = ContentAlignment.MiddleLeft });
        tagSearchRow.Controls.Add(_tagFilterBox);
        leftLayout.Controls.Add(tagSearchRow, 0, 1);

        var cardLists = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        cardLists.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        cardLists.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        cardLists.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        var defaultCardsGroup = new GroupBox { Text = "기본 카드 폴더", Dock = DockStyle.Fill, Padding = new Padding(4) };
        _defaultCardList.Dock = DockStyle.Fill;
        _customCardList.Dock = DockStyle.Fill;
        defaultCardsGroup.Controls.Add(_defaultCardList);
        _customCardsGroup.Controls.Add(_customCardList);
        var moveRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(108, 0, 0, 0) };
        moveRow.Controls.Add(_moveToCustomButton);
        moveRow.Controls.Add(_moveToDefaultButton);
        moveRow.Controls.Add(_deleteCardButton);
        cardLists.Controls.Add(defaultCardsGroup, 0, 0);
        cardLists.Controls.Add(moveRow, 0, 1);
        cardLists.Controls.Add(_customCardsGroup, 0, 2);
        leftLayout.Controls.Add(cardLists, 0, 2);
        left.Controls.Add(leftLayout);
        root.Controls.Add(left, 0, 1);

        var right = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        root.Controls.Add(right, 1, 1);

        right.Controls.Add(_fileNameLabel, 0, 0);
        _tabs.Dock = DockStyle.Fill;
        right.Controls.Add(_tabs, 0, 1);

        var textPage = new TabPage("실제 텍스트 필드");
        var textLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(4) };
        textLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 162));
        textLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var mainTextGroup = new GroupBox { Text = "주요 텍스트", Dock = DockStyle.Fill };
        var mainTextLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(8) };
        mainTextLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        mainTextLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        mainTextLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        mainTextLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        mainTextLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        mainTextLayout.Controls.Add(MakeLabel("파일명"), 0, 0);
        mainTextLayout.Controls.Add(_fileNameBox, 1, 0);
        mainTextLayout.Controls.Add(MakeLabel("카드 이름"), 0, 1);
        mainTextLayout.Controls.Add(_cardNameBox, 1, 1);
        mainTextLayout.Controls.Add(MakeLabel("특수 링크"), 0, 2);
        mainTextLayout.Controls.Add(_evolutionSummaryBox, 1, 2);
        mainTextGroup.Controls.Add(mainTextLayout);
        textLayout.Controls.Add(mainTextGroup, 0, 0);

        _textGrid.Dock = DockStyle.Fill;
        _textGrid.AllowUserToAddRows = false;
        _textGrid.AllowUserToDeleteRows = false;
        _textGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _textGrid.RowHeadersVisible = false;
        _textGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _textGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Field", HeaderText = "필드명", FillWeight = 34, ReadOnly = true });
        _textGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Value", HeaderText = "값", FillWeight = 66 });
        textLayout.Controls.Add(_textGrid, 0, 1);
        textPage.Controls.Add(textLayout);
        _tabs.TabPages.Add(textPage);

        var flavorPage = new TabPage("플레이버/TAG");
        var flavorLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        flavorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        flavorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        flavorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var flavorHeader = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        flavorHeader.Controls.Add(new Label { Text = "카드 플레이버 텍스트", AutoSize = false, Width = 180, Height = 28, TextAlign = ContentAlignment.MiddleLeft });
        flavorHeader.Controls.Add(_autoFlavorButton);
        flavorHeader.Controls.Add(_flavorCountLabel);
        flavorLayout.Controls.Add(flavorHeader, 0, 0);
        var tagRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 1 };
        tagRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64));
        tagRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        tagRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64));
        tagRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        tagRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64));
        tagRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        tagRow.Controls.Add(new Label { Text = "Tag 1", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        tagRow.Controls.Add(_tagTextBox1, 1, 0);
        tagRow.Controls.Add(new Label { Text = "Tag 2", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 0);
        tagRow.Controls.Add(_tagTextBox2, 3, 0);
        tagRow.Controls.Add(new Label { Text = "Tag 3", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 4, 0);
        tagRow.Controls.Add(_tagTextBox3, 5, 0);
        flavorLayout.Controls.Add(tagRow, 0, 1);
        _flavorTextBox.Dock = DockStyle.Fill;
        flavorLayout.Controls.Add(_flavorTextBox, 0, 2);
        flavorPage.Controls.Add(flavorLayout);
        _tabs.TabPages.Add(flavorPage);

        var evolutionPage = new TabPage("특수 링크");
        _evolutionGrid.Dock = DockStyle.Fill;
        _evolutionGrid.AllowUserToAddRows = false;
        _evolutionGrid.AllowUserToDeleteRows = false;
        _evolutionGrid.ReadOnly = true;
        _evolutionGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _evolutionGrid.RowHeadersVisible = false;
        _evolutionGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _evolutionGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Kind", HeaderText = "종류", FillWeight = 18 });
        _evolutionGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "File", HeaderText = "대상 파일명", FillWeight = 30 });
        _evolutionGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "대상 인게임 이름", FillWeight = 30 });
        _evolutionGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Uuid", HeaderText = "대상 UUID", FillWeight = 36 });
        _evolutionGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Updated", HeaderText = "갱신일", FillWeight = 26 });
        evolutionPage.Controls.Add(_evolutionGrid);
        _tabs.TabPages.Add(evolutionPage);

        var imagePage = new TabPage("이미지/BGM bytes");
        _imagePage = imagePage;
        var imagePageLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(4) };
        imagePageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        imagePageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 122));
        imagePageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _imageSizeHintLabel.Text = "권장 이미지 크기: 630 x 880 px (9:13)";
        _imageSizeHintLabel.ForeColor = Color.DarkSlateGray;
        imagePageLayout.Controls.Add(_imageSizeHintLabel, 0, 0);
        imagePageLayout.Controls.Add(BuildGlobalImageEditControls(), 0, 1);
        var imageGrid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(4) };
        imageGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        imageGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        imageGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        imageGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        for (var i = 0; i < _imagePanels.Length; i++)
            imageGrid.Controls.Add(_imagePanels[i], i % 2, i / 2);
        imagePageLayout.Controls.Add(imageGrid, 0, 2);
        imagePage.Controls.Add(imagePageLayout);
        _tabs.TabPages.Add(imagePage);

        var jsonPage = new TabPage("고급 JSON");
        _jsonBox.Dock = DockStyle.Fill;
        jsonPage.Controls.Add(_jsonBox);
        _tabs.TabPages.Add(jsonPage);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        actions.Controls.Add(_applyTextButton);
        actions.Controls.Add(_applyJsonButton);
        _actionNoteLabel.Text = "저장 시 복호화/압축해제/JSON 파싱 검증 후 .card를 생성합니다.\n본 에디터를 사용하여 오는 불이익에 대해선 제작자 KaZaRI는 책임을 지지 않습니다.";
        actions.Controls.Add(_actionNoteLabel);
        right.Controls.Add(actions, 0, 2);

        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        var languageButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        languageButtons.Controls.Add(_langKrButton);
        languageButtons.Controls.Add(_langJpButton);
        languageButtons.Controls.Add(_langEnButton);
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        bottom.Controls.Add(_statusLabel, 0, 0);
        bottom.Controls.Add(languageButtons, 1, 0);
        root.SetColumnSpan(bottom, 2);
        root.Controls.Add(bottom, 0, 2);

        CaptureLocalizationKeys(this);
        ApplyDarkTheme();
        ApplyLanguage(_language);
    }

    private static Label MakeLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            AutoSize = false,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(3, 0, 6, 0),
        };
    }

    private Control BuildGlobalImageEditControls()
    {
        ResetComboItems(_globalDotStyleBox, 0, "압축 없음", "x512", "x258", "x128", "x64", "x32", "x16");

        var group = new GroupBox { Text = "image1/2/3 공통 이미지 조절", Dock = DockStyle.Fill, Padding = new Padding(8) };
        var rows = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        rows.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        rows.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        rows.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        var firstRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        firstRow.Controls.Add(new Label { Text = "압축률", AutoSize = false, Width = 56, Height = 24, TextAlign = ContentAlignment.MiddleLeft });
        firstRow.Controls.Add(_globalDotStyleBox);
        firstRow.Controls.Add(_globalResetImageEditsButton);
        firstRow.Controls.Add(_globalCompressionHintLabel);

        var secondRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        secondRow.Controls.Add(_globalChromaticLabel);
        secondRow.Controls.Add(_globalChromaticTrack);
        secondRow.Controls.Add(_globalHueLabel);
        secondRow.Controls.Add(_globalHueTrack);
        secondRow.Controls.Add(_globalSaturationLabel);
        secondRow.Controls.Add(_globalSaturationTrack);

        var thirdRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        thirdRow.Controls.Add(_globalScaleLabel);
        thirdRow.Controls.Add(_globalScaleTrack);
        thirdRow.Controls.Add(_globalScaleNumber);
        thirdRow.Controls.Add(_globalScaleHintLabel);

        rows.Controls.Add(firstRow, 0, 0);
        rows.Controls.Add(secondRow, 0, 1);
        rows.Controls.Add(thirdRow, 0, 2);
        group.Controls.Add(rows);

        _globalDotStyleBox.SelectedIndexChanged += (_, _) => ApplyGlobalImageEdits();
        _globalChromaticTrack.ValueChanged += (_, _) => ApplyGlobalImageEdits();
        _globalHueTrack.ValueChanged += (_, _) => ApplyGlobalImageEdits();
        _globalSaturationTrack.ValueChanged += (_, _) => ApplyGlobalImageEdits();
        _globalScaleTrack.ValueChanged += (_, _) => SyncGlobalScaleFromTrack();
        _globalScaleNumber.ValueChanged += (_, _) => SyncGlobalScaleFromNumber();
        _globalResetImageEditsButton.Click += (_, _) => ResetGlobalImageEdits();
        UpdateGlobalImageEditLabels();

        return group;
    }

    private void CaptureLocalizationKeys(Control root)
    {
        if (root is Button or Label or GroupBox or TabPage or Form or CheckBox)
        {
            if (root.Tag is null && !string.IsNullOrWhiteSpace(root.Text))
                root.Tag = root.Text;
        }

        foreach (Control child in root.Controls)
            CaptureLocalizationKeys(child);

        foreach (var grid in new[] { _textGrid, _evolutionGrid })
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column.Tag is null && !string.IsNullOrWhiteSpace(column.HeaderText))
                    column.Tag = column.HeaderText;
            }
        }
    }

    private void ApplyLanguage(string language)
    {
        _language = language;
        ApplyLanguageToControl(this);

        foreach (var grid in new[] { _textGrid, _evolutionGrid })
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column.Tag is string key)
                    column.HeaderText = T(key);
            }
        }

        _langEnButton.BackColor = language == "EN" ? UiTheme.Selection : UiTheme.Accent;
        _langJpButton.BackColor = language == "JP" ? UiTheme.Selection : UiTheme.Accent;
        _langKrButton.BackColor = language == "KR" ? UiTheme.Selection : UiTheme.Accent;
        UpdateLocalizedComboItems();
        UpdateGlobalImageEditLabels();
        foreach (var panel in _imagePanels)
            panel.ApplyLanguage(language);
        _imagePreviewForm?.ApplyLanguage(language);
        RenderCurrentCardTextOnly();
    }

    private void ApplyLanguageToControl(Control root)
    {
        if (root.Tag is string key)
            root.Text = T(key);

        foreach (Control child in root.Controls)
            ApplyLanguageToControl(child);
    }

    private string T(string key)
    {
        return LocalizedText.Translate(_language, key);
    }

    private void UpdateLocalizedComboItems()
    {
        ResetComboItems(_cardFilterModeBox, _cardFilterModeBox.SelectedIndex, "레어도 순", "ㄱ~ㄴ / A~Z / あ~順", "Tag 순");
        ResetComboItems(_globalDotStyleBox, _globalDotStyleBox.SelectedIndex, "압축 없음", "x512", "x258", "x128", "x64", "x32", "x16");
    }

    private void ResetComboItems(ComboBox comboBox, int selectedIndex, params string[] itemKeys)
    {
        comboBox.BeginUpdate();
        try
        {
            comboBox.Items.Clear();
            foreach (var key in itemKeys)
                comboBox.Items.Add(T(key));
            comboBox.SelectedIndex = comboBox.Items.Count == 0 ? -1 : Math.Clamp(selectedIndex, 0, comboBox.Items.Count - 1);
        }
        finally
        {
            comboBox.EndUpdate();
        }
    }

    private void ResetGlobalImageEdits()
    {
        ResetGlobalImageEditsWithoutApply();
        ApplyGlobalImageEdits();
    }

    private void ResetGlobalImageEditsWithoutApply()
    {
        _suppressGlobalImageEdits = true;
        _globalDotStyleBox.SelectedIndex = 0;
        _globalChromaticTrack.Value = 0;
        _globalHueTrack.Value = 0;
        _globalSaturationTrack.Value = 0;
        _globalScaleTrack.Value = 100;
        _globalScaleNumber.Value = 100;
        _suppressGlobalImageEdits = false;
        UpdateGlobalImageEditLabels();
    }

    private void ResetGlobalScaleWithoutApply()
    {
        _suppressGlobalImageEdits = true;
        _globalScaleTrack.Value = 100;
        _globalScaleNumber.Value = 100;
        _suppressGlobalImageEdits = false;
        UpdateGlobalImageEditLabels();
    }

    private void ResetImageEditsAfterSave()
    {
        ResetNonDotGlobalImageEditsWithoutApply();
        var settings = CurrentImageEditSettings();
        foreach (var panel in _imagePanels)
        {
            if (panel.FieldName != "bgm")
            {
                panel.ResetPostEffectsWithoutApply();
                panel.SetEditSettings(settings, raiseEvent: false);
            }
        }
        UpdateImagePreviewPopup();
    }

    private void ResetNonDotGlobalImageEditsWithoutApply()
    {
        _suppressGlobalImageEdits = true;
        _globalChromaticTrack.Value = 0;
        _globalHueTrack.Value = 0;
        _globalSaturationTrack.Value = 0;
        _globalScaleTrack.Value = 100;
        _globalScaleNumber.Value = 100;
        _suppressGlobalImageEdits = false;
        UpdateGlobalImageEditLabels();
    }

    private void ApplyGlobalImageEdits()
    {
        if (_suppressGlobalImageEdits) return;

        UpdateGlobalImageEditLabels();
        var settings = CurrentImageEditSettings();
        foreach (var panel in _imagePanels)
        {
            if (panel.FieldName != "bgm")
                panel.SetEditSettings(settings, raiseEvent: true);
        }
    }

    private ImageEditSettings CurrentImageEditSettings()
    {
        var dotSize = _globalDotStyleBox.SelectedIndex switch
        {
            1 => 512,
            2 => 258,
            3 => 128,
            4 => 64,
            5 => 32,
            6 => 16,
            _ => 0,
        };
        return new ImageEditSettings(
            dotSize,
            _globalChromaticTrack.Value,
            _globalHueTrack.Value,
            _globalSaturationTrack.Value,
            _globalScaleTrack.Value,
            PostEffectChain.Default);
    }

    private void UpdateGlobalImageEditLabels()
    {
        _globalChromaticLabel.Text = $"{T("색수차")} {_globalChromaticTrack.Value}";
        _globalHueLabel.Text = $"{T("휴")} {_globalHueTrack.Value}";
        _globalSaturationLabel.Text = $"{T("새츄레이션")} {_globalSaturationTrack.Value}";
        _globalScaleLabel.Text = $"{T("확대/축소")} {_globalScaleTrack.Value}%";
        _globalCompressionHintLabel.Text = T("파일 이미지 용량을 위해 x512를 추천");
        _globalScaleHintLabel.Text = T("630x880 (9:13)의 이미지일 경우 70%로 수정을 권장");
    }

    private void SyncGlobalScaleFromTrack()
    {
        if (_syncingGlobalScale) return;
        _syncingGlobalScale = true;
        if (_globalScaleNumber.Value != _globalScaleTrack.Value)
            _globalScaleNumber.Value = _globalScaleTrack.Value;
        _syncingGlobalScale = false;
        ApplyGlobalImageEdits();
    }

    private void SyncGlobalScaleFromNumber()
    {
        if (_syncingGlobalScale) return;
        _syncingGlobalScale = true;
        var value = (int)_globalScaleNumber.Value;
        if (_globalScaleTrack.Value != value)
            _globalScaleTrack.Value = value;
        _syncingGlobalScale = false;
        ApplyGlobalImageEdits();
    }

    private static PostEffectKind CurrentPostEffect(ComboBox comboBox)
    {
        return comboBox.SelectedIndex switch
        {
            1 => PostEffectKind.LensFlare,
            2 => PostEffectKind.Bloom,
            3 => PostEffectKind.DepthOfField,
            4 => PostEffectKind.Blur,
            5 => PostEffectKind.FilmGrain,
            6 => PostEffectKind.Noise,
            7 => PostEffectKind.Vignette,
            8 => PostEffectKind.Contrast,
            9 => PostEffectKind.Monochrome,
            10 => PostEffectKind.CrtScanlines,
            11 => PostEffectKind.Pixelation,
            12 => PostEffectKind.Halftone,
            13 => PostEffectKind.Glitch,
            14 => PostEffectKind.RgbSplit,
            15 => PostEffectKind.TvStatic,
            16 => PostEffectKind.Interlace,
            _ => PostEffectKind.None,
        };
    }

    private void RenderCurrentCardTextOnly()
    {
        if (_current is null)
        {
            _fileNameLabel.Text = T("파일명: -");
            _imageSizeHintLabel.Text = $"{T("권장 이미지 크기")}: {CardTextureWidth} x {CardTextureHeight} px (9:13)";
            return;
        }

        _fileNameLabel.Text = $"{T("파일명")}: {Path.GetFileName(_current.Path)}";
        RenderEvolutionInfo();
        RenderImages();
    }

    private void WireEvents()
    {
        _chooseFolderButton.Click += (_, _) => ChooseFolder();
        _useDefaultGameFolderButton.Click += (_, _) => UseDefaultGameFolder();
        _chooseDefaultsButton.Click += (_, _) => ChooseDefaults();
        _reloadButton.Click += (_, _) => ReloadCards();
        _defaultCardList.SelectedIndexChanged += (_, _) => OpenSelectedCard(_defaultCardList, _customCardList);
        _customCardList.SelectedIndexChanged += (_, _) => OpenSelectedCard(_customCardList, _defaultCardList);
        _moveToCustomButton.Click += (_, _) => MoveSelectedCard(_defaultCardList, CustomCardsFolder(), "지정 카드 폴더");
        _moveToDefaultButton.Click += (_, _) => MoveSelectedCard(_customCardList, GetDefaultCardsFolder(), "기본 카드 폴더");
        _deleteCardButton.Click += (_, _) => DeleteSelectedCard();
        _cardFilterEnabledBox.CheckedChanged += (_, _) => RebuildCardListKeepingSelection();
        _cardFilterModeBox.SelectedIndexChanged += (_, _) => RebuildCardListKeepingSelection();
        _tagFilterBox.SelectedIndexChanged += (_, _) =>
        {
            if (!_updatingTagFilter)
                RebuildCardListKeepingSelection();
        };
        _applyTextButton.Click += (_, _) => ApplyGridToJson();
        _applyJsonButton.Click += (_, _) => ApplyJsonText();
        _autoFlavorButton.Click += (_, _) => AutoFillFlavorText();
        _flavorTextBox.TextChanged += (_, _) => UpdateFlavorTextCount();
        _saveButton.Click += (_, _) => SaveCurrentCard();
        _fileNameBox.TextChanged += (_, _) => UpdateCurrentDisplay();
        _cardNameBox.TextChanged += (_, _) => UpdateCurrentDisplay();
        _langEnButton.Click += (_, _) => ApplyLanguage("EN");
        _langJpButton.Click += (_, _) => ApplyLanguage("JP");
        _langKrButton.Click += (_, _) => ApplyLanguage("KR");
        foreach (var panel in _imagePanels)
        {
            panel.ReplaceRequested += ReplaceImage;
            panel.ExportRequested += ExportImage;
            panel.ProcessedBytesChanged += ApplyProcessedImageBytes;
        }
    }

    private void ApplyDarkTheme()
    {
        UiTheme.Apply(this);
        UiTheme.ApplyGrid(_textGrid);
        UiTheme.ApplyGrid(_evolutionGrid);
        UiTheme.ApplyTabs(_tabs);

        _currentFileDisplayBox.BackColor = UiTheme.FileBack;
        _currentFileDisplayBox.ForeColor = UiTheme.FileFore;
        _currentGameNameDisplayBox.BackColor = UiTheme.GameNameBack;
        _currentGameNameDisplayBox.ForeColor = UiTheme.GameNameFore;
        _evolutionSummaryBox.BackColor = UiTheme.EvolutionBack;
        _evolutionSummaryBox.ForeColor = UiTheme.EvolutionFore;
        _imageSizeHintLabel.ForeColor = UiTheme.AccentText;
        _statusLabel.ForeColor = UiTheme.Success;
    }

    private void UseDefaultGameFolder()
    {
        var cardsFolder = GetDefaultCardsFolder();
        if (!Directory.Exists(cardsFolder))
        {
            SetStatus($"기본 폴더를 찾을 수 없습니다: {cardsFolder}", true);
            return;
        }

        _folderBox.Text = "";
        ReloadCards();
        SetStatus($"기본 폴더 사용: {cardsFolder}");
    }

    private static string GetDefaultGameMonsterCardsFolder()
    {
        var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(profile, "AppData", "LocalLow", "GasaiGames", "MonsterCards");
    }

    private static string GetDefaultCardsFolder()
    {
        return ResolveCardsFolder(GetDefaultGameMonsterCardsFolder());
    }

    private static string ResolveCardsFolder(string folder)
    {
        var cardsChild = Path.Combine(folder, "Cards");
        return Directory.Exists(cardsChild) ? cardsChild : folder;
    }

    private void ChooseFolder()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "MonsterCards\\Cards 폴더를 선택하세요.",
            UseDescriptionForTitle = true,
        };
        if (Directory.Exists(_folderBox.Text)) dialog.SelectedPath = _folderBox.Text;
        else if (Directory.Exists(GetDefaultCardsFolder())) dialog.SelectedPath = GetDefaultCardsFolder();
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _folderBox.Text = ResolveCardsFolder(dialog.SelectedPath);
        ReloadCards();
    }

    private string CustomCardsFolder() => _folderBox.Text.Trim();

    private void MoveSelectedCard(ListBox sourceList, string targetFolder, string targetLabel)
    {
        if (sourceList.SelectedItem is not CardListItem item)
        {
            SetStatus("이동할 카드를 먼저 선택하세요.", true);
            return;
        }

        if (string.IsNullOrWhiteSpace(targetFolder) || !Directory.Exists(targetFolder))
        {
            SetStatus($"{targetLabel}가 지정되어 있지 않습니다.", true);
            return;
        }

        try
        {
            var sourcePath = Path.GetFullPath(item.Path);
            var targetPath = Path.GetFullPath(Path.Combine(targetFolder, Path.GetFileName(sourcePath)));
            if (PathsEqual(sourcePath, targetPath))
            {
                SetStatus("이미 대상 폴더에 있는 카드입니다.", true);
                return;
            }

            if (File.Exists(targetPath))
            {
                var result = MessageBox.Show(
                    this,
                    $"대상 폴더에 같은 이름의 카드가 있습니다.\n덮어쓰고 이동할까요?\n\n{Path.GetFileName(targetPath)}",
                    "카드 이동 확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
                File.Delete(targetPath);
            }

            File.Move(sourcePath, targetPath);
            if (_current is not null && PathsEqual(_current.Path, sourcePath))
                _current = _current with { Path = targetPath };
            ReloadCards(targetPath);
            SetStatus($"카드 이동 완료: {Path.GetFileName(targetPath)} -> {targetLabel}");
        }
        catch (Exception ex)
        {
            SetStatus($"카드 이동 실패: {ex.Message}", true);
        }
    }

    private void DeleteSelectedCard()
    {
        var selectedPath = SelectedCardPath();
        if (string.IsNullOrWhiteSpace(selectedPath) || !File.Exists(selectedPath))
        {
            SetStatus("제거할 카드를 먼저 선택하세요.", true);
            return;
        }

        var fileName = Path.GetFileName(selectedPath);
        var result = MessageBox.Show(
            this,
            $"{fileName}\n제거하시겠습니까?",
            "카드 제거",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (result != DialogResult.Yes) return;

        try
        {
            File.Delete(selectedPath);
            if (_current is not null && PathsEqual(_current.Path, selectedPath))
            {
                _current = null;
                SetEditorEnabled(false);
                _fileNameLabel.Text = "파일명: -";
                _fileNameBox.Text = "";
                _cardNameBox.Text = "";
                _tagTextBox1.Text = "";
                _tagTextBox2.Text = "";
                _tagTextBox3.Text = "";
                SetFlavorText("");
                _textGrid.Rows.Clear();
                _evolutionGrid.Rows.Clear();
                _jsonBox.Text = "";
                UpdateCurrentDisplay();
            }
            ReloadCards();
            SetStatus($"카드 제거 완료: {fileName}");
        }
        catch (Exception ex)
        {
            SetStatus($"카드 제거 실패: {ex.Message}", true);
        }
    }

    private void ChooseDefaults()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "ES3Defaults.asset 선택",
            Filter = "ES3Defaults.asset|*.asset|모든 파일|*.*",
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _defaultsPath = dialog.FileName;
        _defaultsBox.Text = dialog.FileName;
        SetStatus("ES3Defaults.asset 선택 완료");
    }

    private void ReloadCards(string? selectPath = null)
    {
        _defaultCards.Clear();
        _customCards.Clear();

        try
        {
            _settings = LoadSettings();
        }
        catch
        {
            // Opening a card will report settings errors; listing can still show file names.
        }

        var defaultFolder = GetDefaultCardsFolder();
        LoadCardsFromFolder(defaultFolder, _defaultCards);

        var customFolder = _folderBox.Text.Trim();
        var hasCustomFolder = Directory.Exists(customFolder)
            && !PathsEqual(customFolder, defaultFolder);
        if (hasCustomFolder)
            LoadCardsFromFolder(customFolder, _customCards);

        _customCardsGroup.Visible = hasCustomFolder;
        _moveToCustomButton.Enabled = hasCustomFolder;
        _moveToDefaultButton.Enabled = hasCustomFolder;
        SetStatus($"기본 {_defaultCards.Count}개 / 지정 {_customCards.Count}개의 .card 파일을 찾았습니다.");
        RebuildCardList(selectPath);
    }

    private static void LoadCardsFromFolder(string folder, List<CardListItem> target)
    {
        if (!Directory.Exists(folder)) return;
        foreach (var path in Directory.EnumerateFiles(folder, "*.card", SearchOption.TopDirectoryOnly).OrderBy(Path.GetFileName))
            target.Add(new CardListItem(path));
    }

    private void RebuildCardListKeepingSelection()
    {
        var selectedPath = SelectedCardPath() ?? _current?.Path;
        RebuildCardList(selectedPath);
    }

    private void RebuildCardList(string? selectPath = null)
    {
        UpdateTagSearchState();
        RebuildCardListBox(_defaultCardList, OrderedCards(_defaultCards), selectPath);
        RebuildCardListBox(_customCardList, OrderedCards(_customCards), selectPath);
        if (string.IsNullOrWhiteSpace(selectPath))
            SelectFirstAvailableCard();
    }

    private void RebuildCardListBox(ListBox listBox, IEnumerable<CardListItem> source, string? selectPath)
    {
        listBox.Items.Clear();
        var cards = source.ToList();
        foreach (var card in cards)
            listBox.Items.Add(card);

        if (cards.Count > 0)
        {
            var selectedIndex = -1;
            if (!string.IsNullOrWhiteSpace(selectPath))
            {
                var fullPath = Path.GetFullPath(selectPath);
                var found = cards.FindIndex(card => string.Equals(Path.GetFullPath(card.Path), fullPath, StringComparison.OrdinalIgnoreCase));
                if (found >= 0) selectedIndex = found;
            }
            if (selectedIndex >= 0)
            {
                listBox.SelectedIndex = selectedIndex;
                listBox.TopIndex = Math.Max(0, selectedIndex - 3);
            }
        }
    }

    private string? SelectedCardPath()
    {
        if (_defaultCardList.SelectedItem is CardListItem defaultItem) return defaultItem.Path;
        if (_customCardList.SelectedItem is CardListItem customItem) return customItem.Path;
        return null;
    }

    private void SelectFirstAvailableCard()
    {
        if (_defaultCardList.Items.Count > 0)
        {
            _defaultCardList.SelectedIndex = 0;
            return;
        }

        if (_customCardList.Items.Count > 0)
            _customCardList.SelectedIndex = 0;
    }

    private void UpdateTagSearchState()
    {
        var enabled = _cardFilterEnabledBox.Checked && _cardFilterModeBox.SelectedIndex == 2;
        UpdateAvailableTagFilter();
        _tagFilterBox.Enabled = enabled && _tagFilterBox.Items.Count > 1;
    }

    private IEnumerable<CardListItem> OrderedCards(IEnumerable<CardListItem> source)
    {
        var cards = source.ToList();
        if (!_cardFilterEnabledBox.Checked)
            return cards.OrderBy(card => Path.GetFileName(card.Path), StringComparer.CurrentCultureIgnoreCase);

        return _cardFilterModeBox.SelectedIndex switch
        {
            0 => cards
                .Select(card => new { Card = card, Metadata = ReadCardListMetadata(card.Path) })
                .OrderByDescending(item => item.Metadata.RarityRank)
                .ThenBy(item => item.Metadata.SortName, StringComparer.CurrentCultureIgnoreCase)
                .Select(item => item.Card),
            2 => cards
                .Select(card => new { Card = card, Metadata = ReadCardListMetadata(card.Path) })
                .Select(item => new { item.Card, item.Metadata, DisplayTag = DisplayTagForFilter(item.Metadata.Tags) })
                .OrderBy(item => string.IsNullOrWhiteSpace(item.DisplayTag) ? 1 : 0)
                .ThenBy(item => item.Metadata.SortName, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(item => Path.GetFileName(item.Card.Path), StringComparer.CurrentCultureIgnoreCase)
                .Select(item => string.IsNullOrWhiteSpace(item.DisplayTag) ? item.Card : item.Card with { DisplayTag = item.DisplayTag }),
            _ => cards
                .Select(card => new { Card = card, Metadata = ReadCardListMetadata(card.Path) })
                .OrderBy(item => item.Metadata.SortName, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(item => Path.GetFileName(item.Card.Path), StringComparer.CurrentCultureIgnoreCase)
                .Select(item => item.Card),
        };
    }

    private string SelectedTagFilter()
    {
        if (!_cardFilterEnabledBox.Checked || _cardFilterModeBox.SelectedIndex != 2) return "";
        if (_tagFilterBox.SelectedIndex <= 0) return "";
        return Convert.ToString(_tagFilterBox.SelectedItem)?.Trim() ?? "";
    }

    private string DisplayTagForFilter(IReadOnlyList<string> tags)
    {
        var selectedTag = SelectedTagFilter();
        if (!string.IsNullOrWhiteSpace(selectedTag))
        {
            return tags.FirstOrDefault(tag => string.Equals(tag, selectedTag, StringComparison.CurrentCultureIgnoreCase)) ?? "";
        }

        return tags.FirstOrDefault(tag => !string.IsNullOrWhiteSpace(tag)) ?? "";
    }

    private void UpdateAvailableTagFilter()
    {
        var previous = _tagFilterBox.SelectedIndex > 0
            ? Convert.ToString(_tagFilterBox.SelectedItem)?.Trim() ?? ""
            : "";
        var tags = _defaultCards
            .Concat(_customCards)
            .Select(card => ReadCardListMetadata(card.Path))
            .SelectMany(metadata => metadata.Tags)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(tag => tag, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        _updatingTagFilter = true;
        try
        {
            _tagFilterBox.BeginUpdate();
            _tagFilterBox.Items.Clear();
            _tagFilterBox.Items.Add(T("Tag 선택"));
            foreach (var tag in tags)
                _tagFilterBox.Items.Add(tag);

            var selectedIndex = 0;
            if (!string.IsNullOrWhiteSpace(previous))
            {
                for (var i = 1; i < _tagFilterBox.Items.Count; i++)
                {
                    if (string.Equals(Convert.ToString(_tagFilterBox.Items[i]), previous, StringComparison.CurrentCultureIgnoreCase))
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }
            _tagFilterBox.SelectedIndex = selectedIndex;
        }
        finally
        {
            _tagFilterBox.EndUpdate();
            _updatingTagFilter = false;
        }
    }

    private static bool PathsEqual(string left, string right)
    {
        return string.Equals(Path.GetFullPath(left), Path.GetFullPath(right), StringComparison.OrdinalIgnoreCase);
    }

    private CardListMetadata ReadCardListMetadata(string path)
    {
        try
        {
            var obj = CardCodec.LoadCard(path, _settings);
            var name = FirstNonEmpty(
                CardJson.GetValueText(obj, "cardName"),
                CardJson.GetValueText(obj, "cardRubyName"),
                Path.GetFileNameWithoutExtension(path));
            var rarity = FirstNonEmpty(
                CardJson.GetValueText(obj, "rarity"),
                CardJson.GetValueText(obj, "cardRarity"),
                CardJson.GetValueText(obj, "rare"),
                CardJson.GetValueText(obj, "rank"),
                CardJson.GetValueText(obj, "grade"));
            var tags = CardJson.GetTagValues(obj, CardTagField)
                .Concat(CardJson.GetTagValues(obj, "tag"))
                .Concat(CardJson.GetTagValues(obj, "cardTag"))
                .Concat(CardJson.GetTagValues(obj, "cardTags"))
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
            return new CardListMetadata(name, RarityRank(rarity), tags);
        }
        catch
        {
            return new CardListMetadata(Path.GetFileNameWithoutExtension(path), -1, []);
        }
    }

    private static string FirstNonEmpty(params string[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
        }
        return "";
    }

    private static string RemoveFlavorAttackLine(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        var normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
        var lines = normalized
            .Split('\n')
            .Where(line => !Regex.IsMatch(line, @"^\s*(ATK|Attack|공격력|攻撃力|攻擊力)\s*[:：]", RegexOptions.IgnoreCase))
            .ToArray();

        return string.Join("\r\n", lines).TrimStart('\r', '\n');
    }

    private static string ClampFlavorText(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return text.Length <= FlavorTextLimit ? text : text[..FlavorTextLimit];
    }

    private void SetFlavorText(string text)
    {
        _flavorTextBox.Text = ClampFlavorText(text);
        UpdateFlavorTextCount();
    }

    private void UpdateFlavorTextCount()
    {
        _flavorCountLabel.Text = $"{_flavorTextBox.TextLength}/{FlavorTextLimit}";
        _flavorCountLabel.ForeColor = _flavorTextBox.TextLength >= FlavorTextLimit ? UiTheme.Error : UiTheme.MutedText;
    }

    private static int RarityRank(string rarity)
    {
        if (string.IsNullOrWhiteSpace(rarity)) return -1;
        var text = rarity.Trim().ToLowerInvariant();
        if (int.TryParse(text, out var number)) return number;
        if (text.Contains("mythic") || text.Contains("신화")) return 90;
        if (text.Contains("legend") || text.Contains("전설")) return 80;
        if (text.Contains("ultra") || text.Contains("울트라")) return 70;
        if (text.Contains("super") || text.Contains("슈퍼")) return 60;
        if (text.Contains("epic") || text.Contains("에픽")) return 50;
        if (text.Contains("rare") || text.Contains("레어")) return 40;
        if (text.Contains("uncommon") || text.Contains("언커먼")) return 30;
        if (text.Contains("common") || text.Contains("커먼") || text.Contains("normal") || text.Contains("일반")) return 20;
        return 10;
    }

    private void OpenSelectedCard(ListBox sourceList, ListBox otherList)
    {
        if (sourceList.SelectedItem is not CardListItem item) return;
        if (otherList.SelectedIndex >= 0) otherList.ClearSelected();
        try
        {
            _settings = LoadSettings();
            var obj = CardCodec.LoadCard(item.Path, _settings);
            _current = new LoadedCard(item.Path, obj);
            RenderCurrentCard();
            SetEditorEnabled(true);
            SetStatus($"카드 열기 완료: {Path.GetFileName(item.Path)}");
        }
        catch (Exception ex)
        {
            SetEditorEnabled(false);
            SetStatus($"카드 열기 실패: {ex.Message}", true);
        }
    }

    private CardSettings LoadSettings()
    {
        var settings = CardSettings.Default;
        if (!string.IsNullOrWhiteSpace(_defaultsPath) && File.Exists(_defaultsPath))
        {
            settings = CardSettings.FromDefaultsAsset(File.ReadAllText(_defaultsPath, Encoding.UTF8), settings);
        }

        if (!string.IsNullOrEmpty(_passwordBox.Text))
            settings = settings with { Password = _passwordBox.Text };

        if (settings.Format != 0) throw new InvalidOperationException($"지원하지 않는 ES3 format={settings.Format}");
        if (settings.EncryptionType is not 0 and not 1) throw new InvalidOperationException($"지원하지 않는 encryptionType={settings.EncryptionType}");
        if (settings.CompressionType is not 0 and not 1) throw new InvalidOperationException($"지원하지 않는 compressionType={settings.CompressionType}");
        return settings;
    }

    private void RenderCurrentCard()
    {
        if (_current is null) return;

        _fileNameLabel.Text = $"파일명: {Path.GetFileName(_current.Path)}";
        _fileNameBox.Text = Path.GetFileName(_current.Path);
        RenderTextGrid();
        RenderEvolutionInfo();
        RenderImages();
        _jsonBox.Text = CardCodec.DumpJson(_current.Json);
        UpdateCurrentDisplay();
    }

    private void RenderTextGrid()
    {
        if (_current is null) return;
        _loadingGrid = true;
        var flavorText = ClampFlavorText(RemoveFlavorAttackLine(CardJson.GetStringValue(_current.Json, "flavorText")));
        CardJson.SetStringValue(_current.Json, "flavorText", flavorText);
        _cardNameBox.Text = CardJson.GetStringValue(_current.Json, "cardName");
        var tags = CardJson.GetTagValues(_current.Json, CardTagField);
        _tagTextBox1.Text = tags.ElementAtOrDefault(0) ?? "";
        _tagTextBox2.Text = tags.ElementAtOrDefault(1) ?? "";
        _tagTextBox3.Text = tags.ElementAtOrDefault(2) ?? "";
        SetFlavorText(flavorText);
        _textGrid.Rows.Clear();

        foreach (var field in CardJson.GetEditableTextFields(_current.Json))
        {
            var row = _textGrid.Rows.Add(field.Key, field.Value);
            if (field.Key is "cardName" or "cardRubyName" or "subTitle" or "flavorText")
            {
                _textGrid.Rows[row].DefaultCellStyle.BackColor = UiTheme.HighlightBack;
                _textGrid.Rows[row].DefaultCellStyle.ForeColor = UiTheme.Text;
            }
        }

        _loadingGrid = false;
        UpdateCurrentDisplay();
    }

    private void RenderEvolutionInfo()
    {
        _evolutionGrid.Rows.Clear();
        _evolutionSummaryBox.Text = "none";
        if (_current is null) return;

        var summaries = new List<string>();
        AddEvolutionRows(T("특수 링크"), "ExCardIdList", "ExCardPathList", "ExCardLastUpdateList", summaries);
        AddEvolutionRows(T("특수 링크"), "LinkedCardIdList", "LinkedCardPathList", "LinkedCardLastUpdateList", summaries);

        if (_evolutionGrid.Rows.Count == 0)
        {
            _evolutionGrid.Rows.Add(T("없음"), "", "", "", "");
            _evolutionSummaryBox.Text = "none";
        }
        else
        {
            _evolutionSummaryBox.Text = string.Join(" / ", summaries);
        }
    }

    private void AddEvolutionRows(string kind, string idKey, string pathKey, string updateKey, List<string> summaries)
    {
        if (_current is null) return;

        var ids = CardJson.GetStringList(_current.Json, idKey);
        var paths = CardJson.GetStringList(_current.Json, pathKey);
        var updates = CardJson.GetStringList(_current.Json, updateKey);
        var count = Math.Max(ids.Count, Math.Max(paths.Count, updates.Count));

        for (var i = 0; i < count; i++)
        {
            var id = i < ids.Count ? ids[i] : "";
            var pathValue = i < paths.Count ? paths[i] : "";
            var updated = i < updates.Count ? updates[i] : "";
            var target = ResolveTargetCard(pathValue, id);

            _evolutionGrid.Rows.Add(
                kind,
                target.FileName,
                target.GameName,
                id,
                updated);

            var displayName = string.IsNullOrWhiteSpace(target.GameName)
                ? target.FileName
                : $"{target.GameName} ({target.FileName})";
            summaries.Add($"{kind}: {displayName}");
        }
    }

    private TargetCardInfo ResolveTargetCard(string pathValue, string uuid)
    {
        if (_current is null) return new TargetCardInfo(pathValue, "");

        var folder = Path.GetDirectoryName(_current.Path) ?? "";
        var fileName = Path.GetFileName(pathValue);
        if (!string.IsNullOrWhiteSpace(fileName) && !fileName.EndsWith(".card", StringComparison.OrdinalIgnoreCase))
            fileName += ".card";

        var candidatePath = string.IsNullOrWhiteSpace(fileName) ? "" : Path.Combine(folder, fileName);
        if (!string.IsNullOrWhiteSpace(candidatePath) && File.Exists(candidatePath))
            return ReadTargetCardInfo(candidatePath);

        if (!string.IsNullOrWhiteSpace(uuid))
        {
            foreach (var card in _defaultCards.Concat(_customCards))
            {
                try
                {
                    var obj = CardCodec.LoadCard(card.Path, _settings);
                    if (string.Equals(CardJson.GetStringValue(obj, "cardUUID"), uuid, StringComparison.OrdinalIgnoreCase))
                        return new TargetCardInfo(Path.GetFileName(card.Path), CardJson.GetStringValue(obj, "cardName"));
                }
                catch
                {
                    // Ignore cards that cannot be opened with the current settings.
                }
            }
        }

        return new TargetCardInfo(string.IsNullOrWhiteSpace(fileName) ? pathValue : fileName, "찾을 수 없음");
    }

    private TargetCardInfo ReadTargetCardInfo(string path)
    {
        try
        {
            var obj = CardCodec.LoadCard(path, _settings);
            var name = CardJson.GetStringValue(obj, "cardName");
            return new TargetCardInfo(Path.GetFileName(path), string.IsNullOrWhiteSpace(name) ? "이름 없음" : name);
        }
        catch (Exception ex)
        {
            return new TargetCardInfo(Path.GetFileName(path), $"열기 실패: {ex.Message}");
        }
    }

    private void UpdateCurrentDisplay()
    {
        if (_current is null)
        {
            _currentFileDisplayBox.Text = "none";
            _currentGameNameDisplayBox.Text = "none";
            return;
        }

        var folder = Path.GetDirectoryName(Path.GetFullPath(_current.Path)) ?? "";
        var fileName = NormalizeCardFileName(_fileNameBox.Text, Path.GetFileName(_current.Path));
        _currentFileDisplayBox.Text = Path.Combine(folder, fileName);

        var gameName = _cardNameBox.Text.Trim();
        _currentGameNameDisplayBox.Text = string.IsNullOrEmpty(gameName) ? "none" : gameName;
    }

    private void RenderImages()
    {
        if (_current is null) return;
        var layerSizes = new List<string>();
        foreach (var panel in _imagePanels)
        {
            var bytes = CardJson.TryGetBytes(_current.Json, panel.FieldName, out var data) ? data : null;
            var fileName = panel.FieldName == "bgm" ? CardJson.GetStringValue(_current.Json, "bgmTitle") : null;
            panel.SetBytes(bytes, fileName);
            if (panel.FieldName != "bgm" && bytes is not null && ImagePanel.TryGetPngSize(bytes, out var width, out var height))
                layerSizes.Add($"{panel.FieldName}: {width} x {height} px");
        }
        UpdateImageSizeHint(layerSizes);
        EnsureImagePreviewPopup();
        UpdateImagePreviewPopup();
    }

    private void EnsureImagePreviewPopup()
    {
        if (_imagePreviewForm is { IsDisposed: false }) return;
        _imagePreviewForm = new ImagePreviewForm(_language);
        _imagePreviewForm.FormClosed += (_, _) => _imagePreviewForm = null;
        _imagePreviewForm.Show(this);
        var screen = Screen.FromControl(this).WorkingArea;
        var x = Math.Min(screen.Right - _imagePreviewForm.Width, Right + 12);
        var y = Math.Max(screen.Top, Top);
        _imagePreviewForm.Location = new Point(Math.Max(screen.Left, x), y);
    }

    private void CloseImagePreviewPopup()
    {
        if (_imagePreviewForm is null) return;
        var form = _imagePreviewForm;
        _imagePreviewForm = null;
        if (!form.IsDisposed) form.Close();
    }

    private void UpdateImagePreviewPopup()
    {
        if (_imagePreviewForm is null || _imagePreviewForm.IsDisposed) return;

        var layerBytes = ImageByteFields
            .Reverse()
            .Select(fieldName => _imagePanels.FirstOrDefault(panel => panel.FieldName == fieldName)?.CurrentImageBytes)
            .Where(bytes => bytes is not null)
            .Select(bytes => bytes!)
            .ToList();

        if (layerBytes.Count == 0)
        {
            _imagePreviewForm.SetPreview(null);
            return;
        }

        try
        {
            using var combined = BuildCombinedLayerPreview(layerBytes);
            _imagePreviewForm.SetPreview(combined);
        }
        catch
        {
            _imagePreviewForm.SetPreview(null);
        }
    }

    private static Bitmap BuildCombinedLayerPreview(List<byte[]> layerBytes)
    {
        var images = new List<Image>();
        try
        {
            foreach (var bytes in layerBytes)
            {
                using var ms = new MemoryStream(bytes);
                images.Add(new Bitmap(Image.FromStream(ms)));
            }

            var width = images.Max(image => image.Width);
            var height = images.Max(image => image.Height);
            var combined = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(combined);
            graphics.Clear(Color.Transparent);
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            foreach (var image in images)
                graphics.DrawImage(image, 0, 0, width, height);
            return combined;
        }
        finally
        {
            foreach (var image in images)
                image.Dispose();
        }
    }

    private void UpdateImageSizeHint(List<string> layerSizes)
    {
        var (targetWidth, targetHeight) = GetImageTargetSize("image1Bytes");
        var recommendation = $"{T("권장 이미지 크기")}: {targetWidth} x {targetHeight} px (9:13)";

        if (layerSizes.Count == 0)
        {
            _imageSizeHintLabel.Text = recommendation;
            return;
        }

        var normalizedSizes = layerSizes
            .Select(text => text[(text.IndexOf(':') + 2)..])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        _imageSizeHintLabel.Text = normalizedSizes.Count == 1
            ? recommendation
            : $"{recommendation} / {T("현재 레이어 크기")}: " + string.Join(" / ", layerSizes);
    }

    private void ApplyGridToJson()
    {
        if (_current is null || _loadingGrid) return;

        try
        {
            foreach (DataGridViewRow row in _textGrid.Rows)
            {
                var key = Convert.ToString(row.Cells["Field"].Value) ?? "";
                var value = Convert.ToString(row.Cells["Value"].Value) ?? "";
                if (key.Length > 0) CardJson.SetStringValue(_current.Json, key, value);
            }

            SetFlavorText(RemoveFlavorAttackLine(_flavorTextBox.Text));
            CardJson.SetStringValue(_current.Json, "cardName", _cardNameBox.Text);
            CardJson.SetTagValues(_current.Json, CardTagField, [_tagTextBox1.Text, _tagTextBox2.Text, _tagTextBox3.Text]);
            CardJson.SetStringValue(_current.Json, "flavorText", ClampFlavorText(_flavorTextBox.Text));
            UpdateLastUpdateDate();
            _jsonBox.Text = CardCodec.DumpJson(_current.Json);
            RenderEvolutionInfo();
            UpdateAvailableTagFilter();
            SetStatus("텍스트 필드를 JSON에 적용했습니다.");
        }
        catch (Exception ex)
        {
            SetStatus($"텍스트 적용 실패: {ex.Message}", true);
        }
    }

    private void ApplyJsonText()
    {
        if (_current is null) return;

        try
        {
            var obj = JsonNode.Parse(_jsonBox.Text)?.AsObject() ?? throw new InvalidOperationException("JSON 루트가 비어 있습니다.");
            CardJson.SetStringValue(obj, "flavorText", ClampFlavorText(RemoveFlavorAttackLine(CardJson.GetStringValue(obj, "flavorText"))));
            _current = _current with { Json = obj };
            RenderTextGrid();
            RenderEvolutionInfo();
            RenderImages();
            UpdateAvailableTagFilter();
            SetStatus("JSON을 현재 카드에 적용했습니다.");
        }
        catch (Exception ex)
        {
            SetStatus($"JSON 적용 실패: {ex.Message}", true);
        }
    }

    private void AutoFillFlavorText()
    {
        if (_current is null) return;

        var owner = FirstNonEmpty(CardValue("creatorName", "CreatorName", "owner", "Owner", "creator", "Creator", "author", "Author", "maker", "Maker"), $"[{T("제작자명")}]");
        var tags = CurrentFlavorTags();
        for (var i = 0; i < tags.Length; i++)
            if (string.IsNullOrWhiteSpace(tags[i]))
                tags[i] = "*";
        var separator = "= = = = = = = = = = = = = = = = = = = = = =";
        var text = $"{separator}\r\n카드 테마 : {tags[0]} / {tags[1]} / {tags[2]}\r\n- 제작자 : {owner}\r\n{separator}";

        SetFlavorText(text);
        CardJson.SetStringValue(_current.Json, "flavorText", _flavorTextBox.Text);
        UpdateLastUpdateDate();
        _jsonBox.Text = CardCodec.DumpJson(_current.Json);
        SetStatus(T("플레이버 텍스트 자동 출력 완료"));
    }

    private string[] CurrentFlavorTags()
    {
        if (_current is null) return ["", "", ""];

        var tags = new[]
        {
            _tagTextBox1.Text.Trim(),
            _tagTextBox2.Text.Trim(),
            _tagTextBox3.Text.Trim(),
        };

        if (tags.Any(tag => !string.IsNullOrWhiteSpace(tag)))
            return tags;

        return CardJson.GetTagValues(_current.Json, CardTagField);
    }

    private string CardValue(params string[] keys)
    {
        if (_current is null) return "";

        foreach (var key in keys)
        {
            var value = CardJson.GetValueText(_current.Json, key);
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
        }

        foreach (var pair in _current.Json)
        {
            if (!keys.Any(key => string.Equals(key, pair.Key, StringComparison.OrdinalIgnoreCase))) continue;
            var value = CardJson.GetValueText(_current.Json, pair.Key);
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
        }

        return "";
    }

    private void ApplyProcessedImageBytes(string fieldName, byte[] bytes)
    {
        if (_current is null || fieldName == "bgm") return;

        CardJson.SetBytes(_current.Json, fieldName, bytes);
        UpdateLastUpdateDate();
        _jsonBox.Text = CardCodec.DumpJson(_current.Json);

        var layerSizes = new List<string>();
        foreach (var panel in _imagePanels)
        {
            if (panel.FieldName != "bgm"
                && CardJson.TryGetBytes(_current.Json, panel.FieldName, out var data)
                && ImagePanel.TryGetPngSize(data, out var width, out var height))
            {
                layerSizes.Add($"{panel.FieldName}: {width} x {height} px");
            }
        }
        UpdateImageSizeHint(layerSizes);
        UpdateImagePreviewPopup();
    }

    private void CommitPendingImageEditsForSave()
    {
        if (_current is null) return;

        var committed = false;
        foreach (var panel in _imagePanels)
        {
            if (panel.FieldName == "bgm") continue;
            if (!panel.TryConsumePendingImageBytes(out var bytes)) continue;
            CardJson.SetBytes(_current.Json, panel.FieldName, bytes);
            committed = true;
        }

        if (!committed) return;
        UpdateLastUpdateDate();
        _jsonBox.Text = CardCodec.DumpJson(_current.Json);
    }

    private void ReplaceImage(string fieldName)
    {
        if (_current is null) return;

        using var dialog = new OpenFileDialog
        {
            Title = $"{fieldName} 교체 파일 선택",
            Filter = fieldName == "bgm"
                ? "MP3 파일|*.mp3"
                : "PNG 파일|*.png",
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            if (fieldName == "bgm" && !string.Equals(Path.GetExtension(dialog.FileName), ".mp3", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("BGM은 공식 지원 형식인 MP3만 사용할 수 있습니다.");
            if (fieldName != "bgm" && !string.Equals(Path.GetExtension(dialog.FileName), ".png", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("이미지는 PNG만 사용할 수 있습니다.");

            var bytes = File.ReadAllBytes(dialog.FileName);
            if (fieldName != "bgm" && !ImagePanel.IsPng(bytes))
                throw new InvalidOperationException("선택한 파일은 유효한 PNG가 아닙니다.");
            var normalizeMessage = "";
            if (fieldName == "bgm")
            {
                CardJson.SetBytes(_current.Json, fieldName, bytes);
                CardJson.SetStringValue(_current.Json, "bgmTitle", Path.GetFileName(dialog.FileName));
            }
            else
            {
                bytes = NormalizeReplacementPng(fieldName, bytes, out normalizeMessage);
                ResetGlobalImageEditsWithoutApply();
                CardJson.SetBytes(_current.Json, fieldName, bytes);
            }
            UpdateLastUpdateDate();
            RenderEvolutionInfo();
            RenderImages();
            if (fieldName != "bgm")
                ApplyGlobalImageEdits();
            _jsonBox.Text = CardCodec.DumpJson(_current.Json);
            SetStatus($"{fieldName} 교체 완료: {Path.GetFileName(dialog.FileName)}{normalizeMessage}");
        }
        catch (Exception ex)
        {
            SetStatus($"{fieldName} 교체 실패: {ex.Message}", true);
        }
    }

    private byte[] NormalizeReplacementPng(string fieldName, byte[] bytes, out string message)
    {
        message = "";
        if (!ImagePanel.TryGetPngSize(bytes, out var sourceWidth, out var sourceHeight)) return bytes;
        message = sourceWidth == CardTextureWidth && sourceHeight == CardTextureHeight
            ? $" / 카드 텍스처 크기 유지: {CardTextureWidth} x {CardTextureHeight} px"
            : $" / 카드 텍스처 크기로 변환: {sourceWidth} x {sourceHeight} -> {CardTextureWidth} x {CardTextureHeight} px";
        return ImagePanel.ResizePngToTarget(bytes, CardTextureWidth, CardTextureHeight);
    }

    private int EnsureBlankImageSlots()
    {
        if (_current is null) return 0;

        var filled = 0;
        foreach (var imageField in ImageByteFields)
        {
            if (!NeedsWhiteImageSlot(imageField)) continue;
            var (width, height) = GetImageTargetSize(imageField);
            CardJson.SetBytes(_current.Json, imageField, ImagePanel.CreateSolidPng(width, height, Color.Transparent));
            filled++;
        }
        return filled;
    }

    private bool NeedsWhiteImageSlot(string fieldName)
    {
        if (_current is null) return false;
        if (!CardJson.TryGetBytes(_current.Json, fieldName, out var bytes)) return true;
        return bytes.Length == 0 || !ImagePanel.IsPng(bytes) || !ImagePanel.TryGetPngSize(bytes, out _, out _);
    }

    private (int Width, int Height) GetImageTargetSize(string preferredFieldName)
    {
        return (CardTextureWidth, CardTextureHeight);
    }

    private void ExportImage(string fieldName)
    {
        if (_current is null || !CardJson.TryGetBytes(_current.Json, fieldName, out var bytes)) return;

        using var dialog = new SaveFileDialog
        {
            Title = $"{fieldName} 저장",
            FileName = fieldName == "bgm"
                ? BgmExportFileName(_current.Json, _current.Path)
                : $"{Path.GetFileNameWithoutExtension(_current.Path)}_{fieldName}.png",
            Filter = fieldName == "bgm"
                ? "MP3 파일|*.mp3"
                : "PNG 파일|*.png",
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        File.WriteAllBytes(dialog.FileName, bytes);
        SetStatus($"{fieldName} 저장 완료");
    }

    private void SaveCurrentCard()
    {
        if (_current is null) return;

        try
        {
            ApplyGridToJson();
            var filledImageCount = EnsureBlankImageSlots();
            CommitPendingImageEditsForSave();
            _settings = LoadSettings();
            var targetFileName = NormalizeCardFileName(_fileNameBox.Text, Path.GetFileName(_current.Path));
            var sourcePath = Path.GetFullPath(_current.Path);
            var folder = Path.GetDirectoryName(sourcePath) ?? "";
            var targetPath = Path.GetFullPath(Path.Combine(folder, targetFileName));
            if (!string.Equals(Path.GetDirectoryName(targetPath), folder, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("카드는 원본 폴더 안에만 저장할 수 있습니다.");

            var pathChanged = !string.Equals(sourcePath, targetPath, StringComparison.OrdinalIgnoreCase);
            if (pathChanged && File.Exists(targetPath))
            {
                var result = MessageBox.Show(
                    this,
                    $"이미 같은 이름의 카드가 있습니다.\n덮어쓸까요?\n\n{targetFileName}",
                    "카드 덮어쓰기 확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;
            }

            var bytes = CardCodec.EncodeCard(_current.Json, _settings);
            CardCodec.VerifyRoundTrip(bytes, _settings);
            SaveBytesReplacingOriginal(sourcePath, targetPath, bytes);

            _current = _current with { Path = targetPath };
            _fileNameBox.Text = Path.GetFileName(targetPath);
            _fileNameLabel.Text = $"파일명: {Path.GetFileName(targetPath)}";
            ReloadCards(targetPath);
            UpdateCurrentDisplay();
            ResetImageEditsAfterSave();
            var fillMessage = filledImageCount > 0 ? $" / 빈 이미지 슬롯 {filledImageCount}개를 투명 PNG로 보정" : "";
            SetStatus($"카드 저장 완료: {targetPath}{fillMessage}");
        }
        catch (Exception ex)
        {
            SetStatus($"저장 실패: {ex.Message}", true);
        }
    }

    private void UpdateLastUpdateDate()
    {
        if (_current is null) return;
        if (CardJson.HasStringValue(_current.Json, "LastUpdateDate"))
        {
            CardJson.SetStringValue(_current.Json, "LastUpdateDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }

    private void SetEditorEnabled(bool enabled)
    {
        _textGrid.Enabled = enabled;
        _fileNameBox.Enabled = enabled;
        _cardNameBox.Enabled = enabled;
        _evolutionSummaryBox.Enabled = true;
        _tagTextBox1.Enabled = enabled;
        _tagTextBox2.Enabled = enabled;
        _tagTextBox3.Enabled = enabled;
        _flavorTextBox.Enabled = enabled;
        _currentFileDisplayBox.Enabled = true;
        _currentGameNameDisplayBox.Enabled = true;
        _jsonBox.ReadOnly = !enabled;
        _applyTextButton.Enabled = enabled;
        _applyJsonButton.Enabled = enabled;
        _autoFlavorButton.Enabled = enabled;
        _saveButton.Enabled = enabled;
        foreach (var panel in _imagePanels) panel.Enabled = enabled;
    }

    private void SetStatus(string text, bool isError = false)
    {
        _statusLabel.Text = text;
        _statusLabel.ForeColor = isError ? UiTheme.Error : UiTheme.Success;
    }

    private static string NormalizeCardFileName(string requestedName, string fallbackName)
    {
        var name = string.IsNullOrWhiteSpace(requestedName) ? fallbackName : requestedName.Trim();
        name = Path.GetFileName(name);
        foreach (var invalid in Path.GetInvalidFileNameChars())
            name = name.Replace(invalid, '_');
        if (!name.EndsWith(".card", StringComparison.OrdinalIgnoreCase))
            name += ".card";
        return name;
    }

    private static void SaveBytesReplacingOriginal(string sourcePath, string targetPath, byte[] bytes)
    {
        var tempPath = targetPath + ".tmp";
        File.WriteAllBytes(tempPath, bytes);

        try
        {
            File.Copy(tempPath, targetPath, overwrite: true);
            if (!string.Equals(sourcePath, targetPath, StringComparison.OrdinalIgnoreCase) && File.Exists(sourcePath))
                File.Delete(sourcePath);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private static string BgmExportFileName(JsonObject obj, string cardPath)
    {
        var name = CardJson.GetStringValue(obj, "bgmTitle");
        if (string.IsNullOrWhiteSpace(name))
            name = Path.GetFileNameWithoutExtension(cardPath) + "_bgm.mp3";
        name = Path.GetFileName(name);
        foreach (var invalid in Path.GetInvalidFileNameChars())
            name = name.Replace(invalid, '_');
        if (!name.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            name += ".mp3";
        return name;
    }
}

internal enum PostEffectKind
{
    None,
    LensFlare,
    Bloom,
    DepthOfField,
    Blur,
    FilmGrain,
    Noise,
    Vignette,
    Contrast,
    Monochrome,
    CrtScanlines,
    Pixelation,
    Halftone,
    Glitch,
    RgbSplit,
    TvStatic,
    Interlace,
}

internal sealed record ImageEditSettings(
    int DotSize,
    int ChromaticShift,
    int HueDegrees,
    int SaturationPercent,
    int ScalePercent,
    PostEffectChain PostEffects)
{
    public static ImageEditSettings Default => new(0, 0, 0, 0, 100, PostEffectChain.Default);
}

internal sealed record PostEffectChain(
    PostEffectKind Effect1,
    int Intensity1,
    PostEffectKind Effect2,
    int Intensity2,
    PostEffectKind Effect3,
    int Intensity3)
{
    public static PostEffectChain Default => new(PostEffectKind.None, 60, PostEffectKind.None, 60, PostEffectKind.None, 60);
}

internal sealed class ImagePreviewForm : Form
{
    private readonly PictureBox _preview = new() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(8, 10, 14) };
    private readonly Label _placeholder = new() { Dock = DockStyle.Fill, Text = "레이어 미리보기 없음", TextAlign = ContentAlignment.MiddleCenter };
    private readonly CheckBox _framePreviewBox = new() { Text = "카드 프레임", AutoSize = true };
    private Bitmap? _previewSource;
    private string _language;

    public ImagePreviewForm(string language)
    {
        _language = language;
        Text = Tr("레이어 1+2+3 미리보기");
        Width = 430;
        Height = 640;
        MinimumSize = new Size(280, 380);
        StartPosition = FormStartPosition.Manual;
        BackColor = UiTheme.Background;
        ForeColor = UiTheme.Text;

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 32, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(8, 4, 0, 0) };
        toolbar.Controls.Add(_framePreviewBox);
        Controls.Add(_preview);
        Controls.Add(toolbar);
        _preview.Controls.Add(_placeholder);
        _framePreviewBox.CheckedChanged += (_, _) => RenderPreview();
        UiTheme.Apply(this);
        _placeholder.BackColor = Color.Transparent;
        _placeholder.ForeColor = UiTheme.MutedText;
        _placeholder.Text = Tr("레이어 미리보기 없음");
        _framePreviewBox.Text = Tr("카드 프레임");
    }

    public void ApplyLanguage(string language)
    {
        _language = language;
        Text = Tr("레이어 1+2+3 미리보기");
        _placeholder.Text = Tr("레이어 미리보기 없음");
        _framePreviewBox.Text = Tr("카드 프레임");
    }

    public void SetPreview(Image? image)
    {
        var old = _preview.Image;
        _preview.Image = null;
        old?.Dispose();
        _previewSource?.Dispose();
        _previewSource = null;

        if (image is null)
        {
            _placeholder.Visible = true;
            return;
        }

        _previewSource = new Bitmap(image);
        RenderPreview();
        _placeholder.Visible = false;
    }

    private void RenderPreview()
    {
        if (_previewSource is null) return;
        var old = _preview.Image;
        _preview.Image = _framePreviewBox.Checked
            ? CreateTransparentFramePreview(_previewSource)
            : new Bitmap(_previewSource);
        old?.Dispose();
    }

    private static Bitmap CreateTransparentFramePreview(Image image)
    {
        var result = new Bitmap(image);
        using var graphics = Graphics.FromImage(result);
        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var artRect = CardFrameArtRect(result.Width, result.Height);
        using var framePath = new GraphicsPath { FillMode = FillMode.Alternate };
        framePath.AddRectangle(new Rectangle(0, 0, result.Width, result.Height));
        framePath.AddRectangle(artRect);
        using var frameBrush = new SolidBrush(Color.FromArgb(204, 128, 128, 128));
        graphics.FillPath(frameBrush, framePath);

        using var guidePen = new Pen(Color.FromArgb(220, 255, 255, 255), Math.Max(1, result.Width / 320f));
        graphics.DrawRectangle(guidePen, artRect);
        return result;
    }

    private static Rectangle CardFrameArtRect(int width, int height)
    {
        var x = (int)Math.Round(width * 0.164);
        var y = (int)Math.Round(height * 0.154);
        var w = (int)Math.Round(width * 0.683);
        var h = (int)Math.Round(height * 0.704);
        w = Math.Clamp(w, 1, Math.Max(1, width - x));
        h = Math.Clamp(h, 1, Math.Max(1, height - y));
        return new Rectangle(x, y, w, h);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _previewSource?.Dispose();
        _previewSource = null;
        base.OnFormClosed(e);
    }

    private string Tr(string key) => LocalizedText.Translate(_language, key);
}

internal sealed class ImagePanel : GroupBox
{
    private const int CardTextureWidth = 630;
    private const int CardTextureHeight = 880;

    private static readonly string[] PostEffectOptionKeys =
    [
        "효과 없음",
        "Lens Flare",
        "Bloom",
        "Depth of Field",
        "Blur",
        "Film Grain",
        "Noise",
        "Vignette",
        "Contrast",
        "Monochrome",
        "CRT Scanlines",
        "Pixelation",
        "Halftone",
        "Glitch",
        "RGB Split",
        "TV Static",
        "Interlace",
    ];

    private readonly Label _meta = new() { Dock = DockStyle.Top, Height = 24, TextAlign = ContentAlignment.MiddleLeft };
    private readonly Button _replaceButton = new() { Text = "교체" };
    private readonly Button _exportButton = new() { Text = "추출" };
    private readonly ComboBox _postEffectBox1 = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
    private readonly ComboBox _postEffectBox2 = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
    private readonly ComboBox _postEffectBox3 = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
    private readonly TrackBar _postIntensityTrack1 = new() { Minimum = 0, Maximum = 100, Value = 60, TickFrequency = 25, SmallChange = 5, LargeChange = 20, Width = 120, Height = 30 };
    private readonly TrackBar _postIntensityTrack2 = new() { Minimum = 0, Maximum = 100, Value = 60, TickFrequency = 25, SmallChange = 5, LargeChange = 20, Width = 120, Height = 30 };
    private readonly TrackBar _postIntensityTrack3 = new() { Minimum = 0, Maximum = 100, Value = 60, TickFrequency = 25, SmallChange = 5, LargeChange = 20, Width = 120, Height = 30 };
    private readonly Label _postIntensityLabel1 = new() { AutoSize = false, Width = 68, TextAlign = ContentAlignment.MiddleLeft };
    private readonly Label _postIntensityLabel2 = new() { AutoSize = false, Width = 68, TextAlign = ContentAlignment.MiddleLeft };
    private readonly Label _postIntensityLabel3 = new() { AutoSize = false, Width = 68, TextAlign = ContentAlignment.MiddleLeft };

    private byte[]? _sourceBytes;
    private byte[]? _processedBytes;
    private ImageEditSettings _globalEditSettings = ImageEditSettings.Default;
    private ImageEditSettings _editSettings = ImageEditSettings.Default;
    private bool _suppressPostEffectEvents;
    private bool _hasPendingImageBytes;
    private string _language = "KR";

    public string FieldName { get; }
    public event Action<string>? ReplaceRequested;
    public event Action<string>? ExportRequested;
    public event Action<string, byte[]>? ProcessedBytesChanged;
    public byte[]? CurrentImageBytes => _processedBytes?.ToArray();

    public ImagePanel(string fieldName)
    {
        FieldName = fieldName;
        Text = fieldName;
        Dock = DockStyle.Fill;
        Padding = new Padding(8);

        var isImageLayer = FieldName != "bgm";
        if (isImageLayer)
        {
            InitializePostEffectBox(_postEffectBox1);
            InitializePostEffectBox(_postEffectBox2);
            InitializePostEffectBox(_postEffectBox3);
        }

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = isImageLayer ? 5 : 2, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        if (isImageLayer)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        }
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        buttons.Controls.Add(_replaceButton);
        buttons.Controls.Add(_exportButton);

        layout.Controls.Add(_meta, 0, 0);
        if (isImageLayer)
        {
            layout.Controls.Add(BuildPostEffectRow("후처리1", _postEffectBox1, _postIntensityLabel1, _postIntensityTrack1), 0, 1);
            layout.Controls.Add(BuildPostEffectRow("후처리2", _postEffectBox2, _postIntensityLabel2, _postIntensityTrack2), 0, 2);
            layout.Controls.Add(BuildPostEffectRow("후처리3", _postEffectBox3, _postIntensityLabel3, _postIntensityTrack3), 0, 3);
        }
        layout.Controls.Add(buttons, 0, isImageLayer ? 4 : 1);
        Controls.Add(layout);

        _replaceButton.Click += (_, _) => ReplaceRequested?.Invoke(FieldName);
        _exportButton.Click += (_, _) => ExportRequested?.Invoke(FieldName);
        if (isImageLayer)
        {
            _postEffectBox1.SelectedIndexChanged += (_, _) => RebuildLayerPostEffects();
            _postEffectBox2.SelectedIndexChanged += (_, _) => RebuildLayerPostEffects();
            _postEffectBox3.SelectedIndexChanged += (_, _) => RebuildLayerPostEffects();
            _postIntensityTrack1.ValueChanged += (_, _) => RebuildLayerPostEffects();
            _postIntensityTrack2.ValueChanged += (_, _) => RebuildLayerPostEffects();
            _postIntensityTrack3.ValueChanged += (_, _) => RebuildLayerPostEffects();
            UpdatePostEffectLabels();
        }
    }

    private static void InitializePostEffectBox(ComboBox box)
    {
        box.Items.AddRange(PostEffectOptionKeys.Cast<object>().ToArray());
        box.SelectedIndex = 0;
    }

    public void ApplyLanguage(string language)
    {
        _language = language;
        ResetPostEffectBox(_postEffectBox1, _postEffectBox1.SelectedIndex);
        ResetPostEffectBox(_postEffectBox2, _postEffectBox2.SelectedIndex);
        ResetPostEffectBox(_postEffectBox3, _postEffectBox3.SelectedIndex);
        UpdatePostEffectLabels();
        if (_processedBytes is null && FieldName != "bgm")
            _meta.Text = Tr("미리보기 없음");
    }

    private void ResetPostEffectBox(ComboBox box, int selectedIndex)
    {
        if (box.Items.Count == 0) return;
        box.BeginUpdate();
        try
        {
            box.Items.Clear();
            foreach (var key in PostEffectOptionKeys)
                box.Items.Add(Tr(key));
            box.SelectedIndex = Math.Clamp(selectedIndex, 0, box.Items.Count - 1);
        }
        finally
        {
            box.EndUpdate();
        }
    }

    private static Control BuildPostEffectRow(string label, ComboBox effectBox, Label intensityLabel, TrackBar intensityTrack)
    {
        var row = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        row.Controls.Add(new Label { Text = label, AutoSize = false, Width = 62, Height = 24, TextAlign = ContentAlignment.MiddleLeft });
        row.Controls.Add(effectBox);
        row.Controls.Add(intensityLabel);
        row.Controls.Add(intensityTrack);
        return row;
    }

    private void RebuildLayerPostEffects()
    {
        if (_suppressPostEffectEvents) return;
        UpdatePostEffectLabels();
        _editSettings = _globalEditSettings with { PostEffects = CurrentLayerPostEffects() };
        RebuildProcessedImage(true);
    }

    public void ResetPostEffectsWithoutApply()
    {
        if (FieldName == "bgm") return;
        _suppressPostEffectEvents = true;
        _postEffectBox1.SelectedIndex = _postEffectBox1.Items.Count > 0 ? 0 : -1;
        _postEffectBox2.SelectedIndex = _postEffectBox2.Items.Count > 0 ? 0 : -1;
        _postEffectBox3.SelectedIndex = _postEffectBox3.Items.Count > 0 ? 0 : -1;
        _postIntensityTrack1.Value = 60;
        _postIntensityTrack2.Value = 60;
        _postIntensityTrack3.Value = 60;
        _suppressPostEffectEvents = false;
        UpdatePostEffectLabels();
    }

    private void UpdatePostEffectLabels()
    {
        _postIntensityLabel1.Text = $"{Tr("강도")} {_postIntensityTrack1.Value}";
        _postIntensityLabel2.Text = $"{Tr("강도")} {_postIntensityTrack2.Value}";
        _postIntensityLabel3.Text = $"{Tr("강도")} {_postIntensityTrack3.Value}";
    }

    private string Tr(string key) => LocalizedText.Translate(_language, key);

    private PostEffectChain CurrentLayerPostEffects()
    {
        return new PostEffectChain(
            SelectedPostEffect(_postEffectBox1),
            _postIntensityTrack1.Value,
            SelectedPostEffect(_postEffectBox2),
            _postIntensityTrack2.Value,
            SelectedPostEffect(_postEffectBox3),
            _postIntensityTrack3.Value);
    }

    private static PostEffectKind SelectedPostEffect(ComboBox comboBox)
    {
        return comboBox.SelectedIndex switch
        {
            1 => PostEffectKind.LensFlare,
            2 => PostEffectKind.Bloom,
            3 => PostEffectKind.DepthOfField,
            4 => PostEffectKind.Blur,
            5 => PostEffectKind.FilmGrain,
            6 => PostEffectKind.Noise,
            7 => PostEffectKind.Vignette,
            8 => PostEffectKind.Contrast,
            9 => PostEffectKind.Monochrome,
            10 => PostEffectKind.CrtScanlines,
            11 => PostEffectKind.Pixelation,
            12 => PostEffectKind.Halftone,
            13 => PostEffectKind.Glitch,
            14 => PostEffectKind.RgbSplit,
            15 => PostEffectKind.TvStatic,
            16 => PostEffectKind.Interlace,
            _ => PostEffectKind.None,
        };
    }

    public void SetBytes(byte[]? bytes, string? currentFileName = null)
    {
        _sourceBytes = null;
        _processedBytes = null;
        _hasPendingImageBytes = false;

        var displayFileName = string.IsNullOrWhiteSpace(currentFileName) ? "none" : currentFileName.Trim();

        if (bytes is null)
        {
            _meta.Text = FieldName == "bgm" ? "현재 파일명: none" : "필드 없음";
            _exportButton.Enabled = false;
            return;
        }

        var extension = FieldName == "bgm" ? ".mp3" : DetectExtension(bytes);
        _meta.Text = FieldName == "bgm"
            ? $"현재 파일명: {displayFileName} ({bytes.Length:N0} bytes)"
            : $"{bytes.Length:N0} bytes, {extension}";
        _exportButton.Enabled = true;

        try
        {
            if (IsAudioExtension(extension))
                throw new InvalidOperationException("Audio bytes do not have an image preview.");
            _sourceBytes = bytes.ToArray();
            RebuildProcessedImage(false);
            _hasPendingImageBytes = NeedsCardTextureNormalization(_sourceBytes);
        }
        catch
        {
            _processedBytes = null;
            if (FieldName != "bgm")
                _meta.Text = Tr("미리보기 없음");
        }
    }

    public void SetEditSettings(ImageEditSettings settings, bool raiseEvent)
    {
        _globalEditSettings = settings;
        _editSettings = FieldName == "bgm"
            ? settings
            : settings with { PostEffects = CurrentLayerPostEffects() };
        RebuildProcessedImage(raiseEvent);
    }

    private void RebuildProcessedImage(bool raiseEvent)
    {
        if (_sourceBytes is null || FieldName == "bgm") return;

        try
        {
            _processedBytes = ApplyImageEdits(_sourceBytes);
            UpdateImageMeta(_processedBytes);
            if (raiseEvent)
            {
                _hasPendingImageBytes = true;
                ProcessedBytesChanged?.Invoke(FieldName, _processedBytes.ToArray());
            }
        }
        catch (Exception ex)
        {
            _processedBytes = null;
            _meta.Text = $"이미지 처리 실패: {ex.Message}";
        }
    }

    public bool TryConsumePendingImageBytes(out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        if (!_hasPendingImageBytes || _processedBytes is null || FieldName == "bgm")
            return false;

        bytes = _processedBytes.ToArray();
        _hasPendingImageBytes = false;
        return true;
    }

    private void UpdateImageMeta(byte[] bytes)
    {
        var extension = DetectExtension(bytes);
        var sizeText = TryGetPngSize(bytes, out var width, out var height)
            ? $", {width} x {height} px"
            : "";
        _meta.Text = $"{bytes.Length:N0} bytes, {extension}{sizeText}";
    }

    private byte[] ApplyImageEdits(byte[] bytes)
    {
        if (IsIdentityEdit(_editSettings))
        {
            return NeedsCardTextureNormalization(bytes)
                ? ResizePngToTarget(bytes, CardTextureWidth, CardTextureHeight)
                : bytes.ToArray();
        }

        using var input = new MemoryStream(bytes);
        using var loaded = Image.FromStream(input);
        using var loadedBitmap = new Bitmap(loaded);
        using var source = loadedBitmap.Width == CardTextureWidth && loadedBitmap.Height == CardTextureHeight
            ? new Bitmap(loadedBitmap)
            : ResizeBitmap(loadedBitmap, CardTextureWidth, CardTextureHeight, InterpolationMode.HighQualityBicubic, PixelOffsetMode.HighQuality);

        using var dotted = _editSettings.DotSize > 0
            ? PixelateToDotSize(source, _editSettings.DotSize)
            : new Bitmap(source);
        using var colored = ApplyColorAdjustments(dotted, _editSettings.ChromaticShift, _editSettings.HueDegrees, _editSettings.SaturationPercent);
        using var effected1 = ApplyPostEffect(colored, _editSettings.PostEffects.Effect1, _editSettings.PostEffects.Intensity1);
        using var effected2 = ApplyPostEffect(effected1, _editSettings.PostEffects.Effect2, _editSettings.PostEffects.Intensity2);
        using var effected3 = ApplyPostEffect(effected2, _editSettings.PostEffects.Effect3, _editSettings.PostEffects.Intensity3);
        using var scaled = ApplyCenteredScale(effected3, _editSettings.ScalePercent);

        using var output = new MemoryStream();
        scaled.Save(output, ImageFormat.Png);
        return output.ToArray();
    }

    private static bool IsIdentityEdit(ImageEditSettings settings)
    {
        return settings.DotSize == 0
            && settings.ChromaticShift == 0
            && settings.HueDegrees == 0
            && settings.SaturationPercent == 0
            && settings.ScalePercent == 100
            && settings.PostEffects.Effect1 == PostEffectKind.None
            && settings.PostEffects.Effect2 == PostEffectKind.None
            && settings.PostEffects.Effect3 == PostEffectKind.None;
    }

    private static bool NeedsCardTextureNormalization(byte[] bytes)
    {
        return !TryGetPngSize(bytes, out var width, out var height)
            || width != CardTextureWidth
            || height != CardTextureHeight;
    }

    private static Bitmap ApplyCenteredScale(Bitmap source, int scalePercent)
    {
        scalePercent = Math.Clamp(scalePercent, 10, 300);
        if (scalePercent == 100) return new Bitmap(source);

        var result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(result);
        graphics.Clear(Color.Transparent);
        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var scale = scalePercent / 100f;
        var width = Math.Max(1, (int)Math.Round(source.Width * scale));
        var height = Math.Max(1, (int)Math.Round(source.Height * scale));
        var x = (source.Width - width) / 2;
        var y = (source.Height - height) / 2;
        graphics.DrawImage(source, new Rectangle(x, y, width, height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel);
        return result;
    }

    private static Bitmap ResizeBitmap(Image source, int width, int height, InterpolationMode interpolation, PixelOffsetMode pixelOffset)
    {
        var result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(result);
        graphics.Clear(Color.Transparent);
        graphics.CompositingMode = CompositingMode.SourceOver;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = interpolation;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = pixelOffset;
        graphics.DrawImage(source, 0, 0, width, height);
        return result;
    }

    private static Bitmap PixelateToDotSize(Bitmap source, int dotSize)
    {
        var smallWidth = source.Width >= source.Height
            ? dotSize
            : Math.Max(1, (int)Math.Round(source.Width * dotSize / (double)source.Height));
        var smallHeight = source.Height >= source.Width
            ? dotSize
            : Math.Max(1, (int)Math.Round(source.Height * dotSize / (double)source.Width));

        using var tiny = ResizeBitmap(source, smallWidth, smallHeight, InterpolationMode.HighQualityBicubic, PixelOffsetMode.HighQuality);
        return ResizeBitmap(tiny, source.Width, source.Height, InterpolationMode.NearestNeighbor, PixelOffsetMode.Half);
    }

    private static Bitmap ApplyPostEffect(Bitmap source, PostEffectKind effect, int intensity)
    {
        var amount = ClampFloat(intensity / 100f, 0f, 1f);
        if (effect == PostEffectKind.None || amount <= 0f) return new Bitmap(source);

        return effect switch
        {
            PostEffectKind.LensFlare => ApplyLensFlare(source, amount),
            PostEffectKind.Bloom => ApplyBloom(source, amount),
            PostEffectKind.DepthOfField => ApplyDepthOfField(source, amount),
            PostEffectKind.Blur => BoxBlur(source, Math.Max(1, (int)Math.Round(10 * amount))),
            PostEffectKind.FilmGrain => ApplyNoise(source, amount, monochrome: true, staticBands: false),
            PostEffectKind.Noise => ApplyNoise(source, amount, monochrome: false, staticBands: false),
            PostEffectKind.Vignette => ApplyVignette(source, amount),
            PostEffectKind.Contrast => ApplyContrast(source, amount),
            PostEffectKind.Monochrome => ApplyMonochrome(source, amount),
            PostEffectKind.CrtScanlines => ApplyScanlines(source, amount, interlace: false),
            PostEffectKind.Pixelation => PixelateByBlock(source, Math.Max(2, (int)Math.Round(2 + 18 * amount))),
            PostEffectKind.Halftone => ApplyHalftone(source, amount),
            PostEffectKind.Glitch => ApplyGlitch(source, amount),
            PostEffectKind.RgbSplit => ApplyRgbSplit(source, Math.Max(1, (int)Math.Round(16 * amount))),
            PostEffectKind.TvStatic => ApplyNoise(source, amount, monochrome: false, staticBands: true),
            PostEffectKind.Interlace => ApplyScanlines(source, amount, interlace: true),
            _ => new Bitmap(source),
        };
    }

    private static Bitmap ApplyLensFlare(Bitmap source, float amount)
    {
        var result = new Bitmap(source);
        using var graphics = Graphics.FromImage(result);
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.CompositingMode = CompositingMode.SourceOver;

        var lightX = source.Width * 0.22f;
        var lightY = source.Height * 0.18f;
        var radius = Math.Max(source.Width, source.Height) * (0.25f + amount * 0.25f);
        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
        {
            path.AddEllipse(lightX - radius, lightY - radius, radius * 2f, radius * 2f);
            using var brush = new PathGradientBrush(path)
            {
                CenterColor = Color.FromArgb((int)(150 * amount), 255, 250, 210),
                SurroundColors = new[] { Color.FromArgb(0, 255, 255, 255) },
            };
            graphics.FillPath(brush, path);
        }

        using var streakPen = new Pen(Color.FromArgb((int)(90 * amount), 255, 245, 190), Math.Max(1f, source.Width * 0.008f));
        graphics.DrawLine(streakPen, lightX - radius * 0.65f, lightY, lightX + radius * 1.2f, lightY);
        graphics.DrawLine(streakPen, lightX, lightY - radius * 0.55f, lightX, lightY + radius * 0.85f);

        for (var i = 1; i <= 4; i++)
        {
            var t = i / 5f;
            var cx = lightX + (source.Width * 0.62f - lightX) * t;
            var cy = lightY + (source.Height * 0.58f - lightY) * t;
            var size = radius * (0.07f + i * 0.025f);
            using var brush = new SolidBrush(Color.FromArgb((int)((70 - i * 10) * amount), 255, 230, 170));
            graphics.FillEllipse(brush, cx - size, cy - size, size * 2f, size * 2f);
        }
        return result;
    }

    private static Bitmap ApplyBloom(Bitmap source, float amount)
    {
        ReadBitmapBytes(source, out var sourceBytes, out var stride);
        var mask = new byte[sourceBytes.Length];
        for (var i = 0; i < sourceBytes.Length; i += 4)
        {
            var brightness = (sourceBytes[i] + sourceBytes[i + 1] + sourceBytes[i + 2]) / 3;
            var glow = ClampInt((int)((brightness - 145) * 2.3f), 0, 255);
            mask[i] = (byte)(sourceBytes[i] * glow / 255);
            mask[i + 1] = (byte)(sourceBytes[i + 1] * glow / 255);
            mask[i + 2] = (byte)(sourceBytes[i + 2] * glow / 255);
            mask[i + 3] = sourceBytes[i + 3];
        }

        var blurred = BoxBlurBytes(mask, source.Width, source.Height, stride, Math.Max(2, (int)Math.Round(12 * amount)));
        var output = new byte[sourceBytes.Length];
        for (var i = 0; i < sourceBytes.Length; i += 4)
        {
            output[i] = (byte)ClampInt(sourceBytes[i] + (int)(blurred[i] * amount * 1.2f), 0, 255);
            output[i + 1] = (byte)ClampInt(sourceBytes[i + 1] + (int)(blurred[i + 1] * amount * 1.2f), 0, 255);
            output[i + 2] = (byte)ClampInt(sourceBytes[i + 2] + (int)(blurred[i + 2] * amount * 1.2f), 0, 255);
            output[i + 3] = sourceBytes[i + 3];
        }
        return CreateBitmapFromBytes(source.Width, source.Height, output, stride);
    }

    private static Bitmap ApplyDepthOfField(Bitmap source, float amount)
    {
        using var blurred = BoxBlur(source, Math.Max(2, (int)Math.Round(12 * amount)));
        ReadBitmapBytes(source, out var sharpBytes, out var stride);
        ReadBitmapBytes(blurred, out var blurBytes, out _);
        var output = new byte[sharpBytes.Length];
        var centerX = (source.Width - 1) / 2f;
        var centerY = (source.Height - 1) / 2f;
        var focusRadius = Math.Min(source.Width, source.Height) * (0.22f + (1f - amount) * 0.15f);
        var falloff = Math.Min(source.Width, source.Height) * 0.28f;

        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var i = y * stride + x * 4;
                var distance = MathF.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                var blend = ClampFloat((distance - focusRadius) / falloff, 0f, 1f) * amount;
                output[i] = BlendByte(sharpBytes[i], blurBytes[i], blend);
                output[i + 1] = BlendByte(sharpBytes[i + 1], blurBytes[i + 1], blend);
                output[i + 2] = BlendByte(sharpBytes[i + 2], blurBytes[i + 2], blend);
                output[i + 3] = sharpBytes[i + 3];
            }
        }
        return CreateBitmapFromBytes(source.Width, source.Height, output, stride);
    }

    private static Bitmap ApplyNoise(Bitmap source, float amount, bool monochrome, bool staticBands)
    {
        ReadBitmapBytes(source, out var bytes, out var stride);
        var random = new Random(12345);
        var strength = staticBands ? 145 * amount : 70 * amount;
        for (var y = 0; y < source.Height; y++)
        {
            var rowNoise = staticBands && y % 7 == 0 ? random.Next(-80, 81) * amount : 0f;
            for (var x = 0; x < source.Width; x++)
            {
                var i = y * stride + x * 4;
                if (monochrome)
                {
                    var noise = (int)(random.NextDouble() * strength * 2 - strength + rowNoise);
                    bytes[i] = (byte)ClampInt(bytes[i] + noise, 0, 255);
                    bytes[i + 1] = (byte)ClampInt(bytes[i + 1] + noise, 0, 255);
                    bytes[i + 2] = (byte)ClampInt(bytes[i + 2] + noise, 0, 255);
                }
                else
                {
                    bytes[i] = (byte)ClampInt(bytes[i] + (int)(random.NextDouble() * strength * 2 - strength + rowNoise), 0, 255);
                    bytes[i + 1] = (byte)ClampInt(bytes[i + 1] + (int)(random.NextDouble() * strength * 2 - strength + rowNoise), 0, 255);
                    bytes[i + 2] = (byte)ClampInt(bytes[i + 2] + (int)(random.NextDouble() * strength * 2 - strength + rowNoise), 0, 255);
                }
            }
        }
        return CreateBitmapFromBytes(source.Width, source.Height, bytes, stride);
    }

    private static Bitmap ApplyVignette(Bitmap source, float amount)
    {
        ReadBitmapBytes(source, out var bytes, out var stride);
        var centerX = (source.Width - 1) / 2f;
        var centerY = (source.Height - 1) / 2f;
        var maxDistance = MathF.Sqrt(centerX * centerX + centerY * centerY);
        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var i = y * stride + x * 4;
                var distance = MathF.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY)) / maxDistance;
                var factor = 1f - ClampFloat((distance - 0.35f) / 0.65f, 0f, 1f) * amount * 0.85f;
                bytes[i] = (byte)(bytes[i] * factor);
                bytes[i + 1] = (byte)(bytes[i + 1] * factor);
                bytes[i + 2] = (byte)(bytes[i + 2] * factor);
            }
        }
        return CreateBitmapFromBytes(source.Width, source.Height, bytes, stride);
    }

    private static Bitmap ApplyContrast(Bitmap source, float amount)
    {
        ReadBitmapBytes(source, out var bytes, out var stride);
        var contrast = 1f + amount * 1.8f;
        for (var i = 0; i < bytes.Length; i += 4)
        {
            bytes[i] = ContrastByte(bytes[i], contrast);
            bytes[i + 1] = ContrastByte(bytes[i + 1], contrast);
            bytes[i + 2] = ContrastByte(bytes[i + 2], contrast);
        }
        return CreateBitmapFromBytes(source.Width, source.Height, bytes, stride);
    }

    private static Bitmap ApplyMonochrome(Bitmap source, float amount)
    {
        ReadBitmapBytes(source, out var bytes, out var stride);
        for (var i = 0; i < bytes.Length; i += 4)
        {
            var gray = (byte)ClampInt((int)(bytes[i + 2] * 0.299f + bytes[i + 1] * 0.587f + bytes[i] * 0.114f), 0, 255);
            bytes[i] = BlendByte(bytes[i], gray, amount);
            bytes[i + 1] = BlendByte(bytes[i + 1], gray, amount);
            bytes[i + 2] = BlendByte(bytes[i + 2], gray, amount);
        }
        return CreateBitmapFromBytes(source.Width, source.Height, bytes, stride);
    }

    private static Bitmap ApplyScanlines(Bitmap source, float amount, bool interlace)
    {
        ReadBitmapBytes(source, out var bytes, out var stride);
        var dark = interlace ? 0.55f : 0.68f;
        for (var y = 0; y < source.Height; y++)
        {
            var apply = interlace ? y % 2 == 1 : y % 3 == 1;
            if (!apply) continue;
            var factor = 1f - (1f - dark) * amount;
            for (var x = 0; x < source.Width; x++)
            {
                var i = y * stride + x * 4;
                bytes[i] = (byte)(bytes[i] * factor);
                bytes[i + 1] = (byte)(bytes[i + 1] * factor);
                bytes[i + 2] = (byte)(bytes[i + 2] * factor);
            }
        }
        return CreateBitmapFromBytes(source.Width, source.Height, bytes, stride);
    }

    private static Bitmap PixelateByBlock(Bitmap source, int blockSize)
    {
        using var tiny = ResizeBitmap(source, Math.Max(1, source.Width / blockSize), Math.Max(1, source.Height / blockSize), InterpolationMode.HighQualityBicubic, PixelOffsetMode.HighQuality);
        return ResizeBitmap(tiny, source.Width, source.Height, InterpolationMode.NearestNeighbor, PixelOffsetMode.Half);
    }

    private static Bitmap ApplyHalftone(Bitmap source, float amount)
    {
        var result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(result);
        graphics.Clear(Color.White);
        var cell = Math.Max(5, (int)Math.Round(14 - amount * 7));
        using var input = new Bitmap(source);
        for (var y = 0; y < source.Height; y += cell)
        {
            for (var x = 0; x < source.Width; x += cell)
            {
                var color = input.GetPixel(Math.Min(source.Width - 1, x + cell / 2), Math.Min(source.Height - 1, y + cell / 2));
                var brightness = (color.R + color.G + color.B) / 3f / 255f;
                var radius = cell * (1f - brightness) * (0.25f + amount * 0.35f);
                using var brush = new SolidBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                graphics.FillEllipse(brush, x + cell / 2f - radius, y + cell / 2f - radius, radius * 2f, radius * 2f);
            }
        }
        if (amount < 1f)
            return BlendBitmaps(source, result, amount);
        return result;
    }

    private static Bitmap ApplyGlitch(Bitmap source, float amount)
    {
        ReadBitmapBytes(source, out var input, out var stride);
        var output = input.ToArray();
        var random = new Random(24680);
        var bands = Math.Max(3, (int)Math.Round(16 * amount));
        for (var b = 0; b < bands; b++)
        {
            var y = random.Next(0, source.Height);
            var height = random.Next(1, Math.Max(2, (int)(source.Height * 0.035f)));
            var shift = random.Next(-(int)(source.Width * 0.08f * amount), (int)(source.Width * 0.08f * amount) + 1);
            for (var yy = y; yy < Math.Min(source.Height, y + height); yy++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var sx = ClampInt(x - shift, 0, source.Width - 1);
                    var from = yy * stride + sx * 4;
                    var to = yy * stride + x * 4;
                    output[to] = input[from];
                    output[to + 1] = (byte)ClampInt(input[from + 1] + random.Next(-30, 31), 0, 255);
                    output[to + 2] = input[from + 2];
                    output[to + 3] = input[from + 3];
                }
            }
        }
        return CreateBitmapFromBytes(source.Width, source.Height, output, stride);
    }

    private static Bitmap ApplyRgbSplit(Bitmap source, int shift)
    {
        ReadBitmapBytes(source, out var input, out var stride);
        var output = new byte[input.Length];
        for (var y = 0; y < source.Height; y++)
        {
            var row = y * stride;
            for (var x = 0; x < source.Width; x++)
            {
                var baseIndex = row + x * 4;
                var redIndex = row + ClampInt(x + shift, 0, source.Width - 1) * 4;
                var blueIndex = row + ClampInt(x - shift, 0, source.Width - 1) * 4;
                output[baseIndex] = input[blueIndex];
                output[baseIndex + 1] = input[baseIndex + 1];
                output[baseIndex + 2] = input[redIndex + 2];
                output[baseIndex + 3] = input[baseIndex + 3];
            }
        }
        return CreateBitmapFromBytes(source.Width, source.Height, output, stride);
    }

    private static Bitmap BoxBlur(Bitmap source, int radius)
    {
        if (radius <= 0) return new Bitmap(source);
        ReadBitmapBytes(source, out var input, out var stride);
        var output = BoxBlurBytes(input, source.Width, source.Height, stride, radius);
        return CreateBitmapFromBytes(source.Width, source.Height, output, stride);
    }

    private static byte[] BoxBlurBytes(byte[] input, int width, int height, int stride, int radius)
    {
        var horizontal = new byte[input.Length];
        var output = new byte[input.Length];
        var window = radius * 2 + 1;

        for (var y = 0; y < height; y++)
        {
            var row = y * stride;
            for (var x = 0; x < width; x++)
            {
                var sumB = 0;
                var sumG = 0;
                var sumR = 0;
                var sumA = 0;
                var count = 0;
                for (var xx = Math.Max(0, x - radius); xx <= Math.Min(width - 1, x + radius); xx++)
                {
                    var i = row + xx * 4;
                    sumB += input[i];
                    sumG += input[i + 1];
                    sumR += input[i + 2];
                    sumA += input[i + 3];
                    count++;
                }
                var target = row + x * 4;
                horizontal[target] = (byte)(sumB / count);
                horizontal[target + 1] = (byte)(sumG / count);
                horizontal[target + 2] = (byte)(sumR / count);
                horizontal[target + 3] = (byte)(sumA / count);
            }
        }

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var sumB = 0;
                var sumG = 0;
                var sumR = 0;
                var sumA = 0;
                var count = 0;
                for (var yy = Math.Max(0, y - radius); yy <= Math.Min(height - 1, y + radius); yy++)
                {
                    var i = yy * stride + x * 4;
                    sumB += horizontal[i];
                    sumG += horizontal[i + 1];
                    sumR += horizontal[i + 2];
                    sumA += horizontal[i + 3];
                    count++;
                }
                var target = y * stride + x * 4;
                output[target] = (byte)(sumB / count);
                output[target + 1] = (byte)(sumG / count);
                output[target + 2] = (byte)(sumR / count);
                output[target + 3] = (byte)(sumA / count);
            }
        }

        _ = window;
        return output;
    }

    private static Bitmap BlendBitmaps(Bitmap a, Bitmap b, float amount)
    {
        ReadBitmapBytes(a, out var aBytes, out var stride);
        ReadBitmapBytes(b, out var bBytes, out _);
        var output = new byte[aBytes.Length];
        for (var i = 0; i < output.Length; i += 4)
        {
            output[i] = BlendByte(aBytes[i], bBytes[i], amount);
            output[i + 1] = BlendByte(aBytes[i + 1], bBytes[i + 1], amount);
            output[i + 2] = BlendByte(aBytes[i + 2], bBytes[i + 2], amount);
            output[i + 3] = aBytes[i + 3];
        }
        return CreateBitmapFromBytes(a.Width, a.Height, output, stride);
    }

    private static void ReadBitmapBytes(Bitmap source, out byte[] bytes, out int stride)
    {
        using var clone = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(clone))
            graphics.DrawImage(source, 0, 0, source.Width, source.Height);

        var bounds = new Rectangle(0, 0, clone.Width, clone.Height);
        var data = clone.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            stride = Math.Abs(data.Stride);
            bytes = new byte[stride * clone.Height];
            var raw = new byte[stride * clone.Height];
            Marshal.Copy(data.Scan0, raw, 0, raw.Length);
            for (var y = 0; y < clone.Height; y++)
            {
                var sourceRow = (data.Stride > 0 ? y : clone.Height - 1 - y) * stride;
                var targetRow = y * stride;
                Array.Copy(raw, sourceRow, bytes, targetRow, stride);
            }
        }
        finally
        {
            clone.UnlockBits(data);
        }
    }

    private static Bitmap CreateBitmapFromBytes(int width, int height, byte[] bytes, int stride)
    {
        var result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        var bounds = new Rectangle(0, 0, width, height);
        var data = result.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        try
        {
            var resultStride = Math.Abs(data.Stride);
            var raw = new byte[resultStride * height];
            for (var y = 0; y < height; y++)
            {
                var sourceRow = y * stride;
                var targetRow = (data.Stride > 0 ? y : height - 1 - y) * resultStride;
                Array.Copy(bytes, sourceRow, raw, targetRow, Math.Min(stride, resultStride));
            }
            Marshal.Copy(raw, 0, data.Scan0, raw.Length);
        }
        finally
        {
            result.UnlockBits(data);
        }
        return result;
    }

    private static byte BlendByte(byte from, byte to, float amount)
    {
        return (byte)ClampInt((int)Math.Round(from + (to - from) * amount), 0, 255);
    }

    private static byte ContrastByte(byte value, float contrast)
    {
        return (byte)ClampInt((int)Math.Round((value - 128) * contrast + 128), 0, 255);
    }

    private static Bitmap ApplyColorAdjustments(Bitmap source, int chromaticShift, int hueDegrees, int saturationPercent)
    {
        if (chromaticShift == 0 && hueDegrees == 0 && saturationPercent == 0)
            return new Bitmap(source);

        var result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
        var bounds = new Rectangle(0, 0, source.Width, source.Height);
        var sourceData = source.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var resultData = result.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        try
        {
            var sourceStride = Math.Abs(sourceData.Stride);
            var resultStride = Math.Abs(resultData.Stride);
            var sourceBytes = new byte[sourceStride * source.Height];
            var resultBytes = new byte[resultStride * result.Height];
            Marshal.Copy(sourceData.Scan0, sourceBytes, 0, sourceBytes.Length);

            for (var y = 0; y < source.Height; y++)
            {
                var sourceRow = (sourceData.Stride > 0 ? y : source.Height - 1 - y) * sourceStride;
                var resultRow = (resultData.Stride > 0 ? y : result.Height - 1 - y) * resultStride;
                for (var x = 0; x < source.Width; x++)
                {
                    var baseIndex = sourceRow + x * 4;
                    var redIndex = chromaticShift > 0
                        ? sourceRow + ClampInt(x + chromaticShift, 0, source.Width - 1) * 4
                        : baseIndex;
                    var blueIndex = chromaticShift > 0
                        ? sourceRow + ClampInt(x - chromaticShift, 0, source.Width - 1) * 4
                        : baseIndex;

                    var adjusted = Color.FromArgb(
                        sourceBytes[baseIndex + 3],
                        sourceBytes[redIndex + 2],
                        sourceBytes[baseIndex + 1],
                        sourceBytes[blueIndex]);

                    if (hueDegrees != 0 || saturationPercent != 0)
                        adjusted = ShiftHueSaturation(adjusted, hueDegrees, saturationPercent);

                    var resultIndex = resultRow + x * 4;
                    resultBytes[resultIndex] = adjusted.B;
                    resultBytes[resultIndex + 1] = adjusted.G;
                    resultBytes[resultIndex + 2] = adjusted.R;
                    resultBytes[resultIndex + 3] = adjusted.A;
                }
            }

            Marshal.Copy(resultBytes, 0, resultData.Scan0, resultBytes.Length);
        }
        finally
        {
            source.UnlockBits(sourceData);
            result.UnlockBits(resultData);
        }
        return result;
    }

    private static Color ShiftHueSaturation(Color color, int hueDegrees, int saturationPercent)
    {
        if (color.A == 0) return color;

        RgbToHsl(color.R, color.G, color.B, out var h, out var s, out var l);
        h = (h + hueDegrees / 360f) % 1f;
        if (h < 0) h += 1f;
        s = ClampFloat(s * (1f + saturationPercent / 100f), 0f, 1f);
        var (r, g, b) = HslToRgb(h, s, l);
        return Color.FromArgb(color.A, r, g, b);
    }

    private static void RgbToHsl(int red, int green, int blue, out float hue, out float saturation, out float lightness)
    {
        var r = red / 255f;
        var g = green / 255f;
        var b = blue / 255f;
        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        lightness = (max + min) / 2f;

        if (Math.Abs(max - min) < 0.0001f)
        {
            hue = 0f;
            saturation = 0f;
            return;
        }

        var delta = max - min;
        saturation = lightness > 0.5f ? delta / (2f - max - min) : delta / (max + min);
        hue = max == r
            ? (g - b) / delta + (g < b ? 6f : 0f)
            : max == g
                ? (b - r) / delta + 2f
                : (r - g) / delta + 4f;
        hue /= 6f;
    }

    private static (int Red, int Green, int Blue) HslToRgb(float hue, float saturation, float lightness)
    {
        if (saturation <= 0f)
        {
            var value = (int)Math.Round(lightness * 255f);
            return (value, value, value);
        }

        var q = lightness < 0.5f
            ? lightness * (1f + saturation)
            : lightness + saturation - lightness * saturation;
        var p = 2f * lightness - q;
        var r = HueToRgb(p, q, hue + 1f / 3f);
        var g = HueToRgb(p, q, hue);
        var b = HueToRgb(p, q, hue - 1f / 3f);
        return (
            ClampInt((int)Math.Round(r * 255f), 0, 255),
            ClampInt((int)Math.Round(g * 255f), 0, 255),
            ClampInt((int)Math.Round(b * 255f), 0, 255));
    }

    private static float HueToRgb(float p, float q, float t)
    {
        if (t < 0f) t += 1f;
        if (t > 1f) t -= 1f;
        if (t < 1f / 6f) return p + (q - p) * 6f * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
        return p;
    }

    private static int ClampInt(int value, int min, int max)
    {
        return Math.Min(max, Math.Max(min, value));
    }

    private static float ClampFloat(float value, float min, float max)
    {
        return Math.Min(max, Math.Max(min, value));
    }

    public static string DetectExtension(byte[] bytes)
    {
        if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return ".png";
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF) return ".jpg";
        if (bytes.Length >= 12 && Encoding.ASCII.GetString(bytes, 0, 4) == "RIFF" && Encoding.ASCII.GetString(bytes, 8, 4) == "WAVE") return ".wav";
        if (bytes.Length >= 4 && Encoding.ASCII.GetString(bytes, 0, 4) == "OggS") return ".ogg";
        if (bytes.Length >= 3 && Encoding.ASCII.GetString(bytes, 0, 3) == "ID3") return ".mp3";
        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] is 0xFB or 0xF3 or 0xF2) return ".mp3";
        if (bytes.Length >= 6)
        {
            var head = Encoding.ASCII.GetString(bytes, 0, 6);
            if (head is "GIF87a" or "GIF89a") return ".gif";
        }
        if (bytes.Length >= 12)
        {
            var riff = Encoding.ASCII.GetString(bytes, 0, 4);
            var webp = Encoding.ASCII.GetString(bytes, 8, 4);
            if (riff == "RIFF" && webp == "WEBP") return ".webp";
        }
        if (bytes.Length >= 12 && Encoding.ASCII.GetString(bytes, 4, 4) == "ftyp") return ".m4a";
        return ".bin";
    }

    public static bool IsPng(byte[] bytes)
    {
        return bytes.Length >= 8
            && bytes[0] == 0x89
            && bytes[1] == 0x50
            && bytes[2] == 0x4E
            && bytes[3] == 0x47
            && bytes[4] == 0x0D
            && bytes[5] == 0x0A
            && bytes[6] == 0x1A
            && bytes[7] == 0x0A;
    }

    public static bool TryGetPngSize(byte[] bytes, out int width, out int height)
    {
        width = 0;
        height = 0;
        if (!IsPng(bytes) || bytes.Length < 24) return false;
        width = ReadBigEndianInt32(bytes, 16);
        height = ReadBigEndianInt32(bytes, 20);
        return width > 0 && height > 0;
    }

    public static byte[] ResizePngToTarget(byte[] bytes, int targetWidth, int targetHeight)
    {
        using var input = new MemoryStream(bytes);
        using var source = Image.FromStream(input);
        using var canvas = new Bitmap(targetWidth, targetHeight, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(canvas))
        {
            graphics.Clear(Color.Transparent);
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graphics.DrawImage(source, 0, 0, targetWidth, targetHeight);
        }

        using var output = new MemoryStream();
        canvas.Save(output, ImageFormat.Png);
        return output.ToArray();
    }

    public static byte[] CreateSolidPng(int width, int height, Color color)
    {
        using var canvas = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(canvas))
        {
            graphics.Clear(color);
        }

        using var output = new MemoryStream();
        canvas.Save(output, ImageFormat.Png);
        return output.ToArray();
    }

    private static int ReadBigEndianInt32(byte[] bytes, int offset)
    {
        return (bytes[offset] << 24)
            | (bytes[offset + 1] << 16)
            | (bytes[offset + 2] << 8)
            | bytes[offset + 3];
    }

    private static bool IsAudioExtension(string extension)
    {
        return extension is ".mp3" or ".ogg" or ".wav" or ".m4a" or ".aac";
    }
}

internal sealed record CardListItem(string Path, string DisplayTag = "")
{
    public override string ToString()
    {
        var fileName = System.IO.Path.GetFileName(Path);
        return string.IsNullOrWhiteSpace(DisplayTag)
            ? fileName
            : $"[{DisplayTag}] {fileName}";
    }
}

internal sealed record CardListMetadata(string SortName, int RarityRank, string[] Tags);

internal sealed record LoadedCard(string Path, JsonObject Json);

internal sealed record TargetCardInfo(string FileName, string GameName);

internal sealed record CardSettings(int EncryptionType, int CompressionType, string Password, int Format)
{
    public static CardSettings Default => new(1, 1, "twoweeks", 0);

    public static CardSettings FromDefaultsAsset(string text, CardSettings fallback)
    {
        return fallback with
        {
            EncryptionType = FindInt(text, "encryptionType", fallback.EncryptionType),
            CompressionType = FindInt(text, "compressionType", fallback.CompressionType),
            Password = FindString(text, "encryptionPassword", fallback.Password),
            Format = FindInt(text, "format", fallback.Format),
        };
    }

    private static int FindInt(string text, string key, int fallback)
    {
        var match = Regex.Match(text, @"^\s*" + Regex.Escape(key) + @"\s*:\s*(-?\d+)\s*$", RegexOptions.Multiline);
        return match.Success ? int.Parse(match.Groups[1].Value) : fallback;
    }

    private static string FindString(string text, string key, string fallback)
    {
        var match = Regex.Match(text, @"^\s*" + Regex.Escape(key) + @"\s*:\s*(.*?)\s*$", RegexOptions.Multiline);
        if (!match.Success) return fallback;
        var value = match.Groups[1].Value.Trim();
        if ((value.StartsWith('"') && value.EndsWith('"')) || (value.StartsWith('\'') && value.EndsWith('\'')))
            value = value[1..^1];
        return value;
    }
}

internal static class CardCodec
{
    public static JsonObject LoadCard(string path, CardSettings settings)
    {
        var cardBytes = File.ReadAllBytes(path);
        var jsonBytes = DecodeCard(cardBytes, settings);
        return JsonNode.Parse(Encoding.UTF8.GetString(jsonBytes))?.AsObject()
            ?? throw new InvalidOperationException("JSON 루트가 비어 있습니다.");
    }

    public static byte[] EncodeCard(JsonObject obj, CardSettings settings)
    {
        var jsonBytes = Encoding.UTF8.GetBytes(DumpJson(obj));
        var data = jsonBytes;
        if (settings.CompressionType == 1) data = GzipCompress(data);
        if (settings.EncryptionType == 1) data = AesEncryptEs3(data, settings.Password);
        return data;
    }

    public static void VerifyRoundTrip(byte[] cardBytes, CardSettings settings)
    {
        var decoded = DecodeCard(cardBytes, settings);
        _ = JsonNode.Parse(Encoding.UTF8.GetString(decoded))?.AsObject()
            ?? throw new InvalidOperationException("생성된 카드의 JSON 검증에 실패했습니다.");
    }

    public static string DumpJson(JsonObject obj)
    {
        return obj.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        }) + Environment.NewLine;
    }

    private static byte[] DecodeCard(byte[] cardBytes, CardSettings settings)
    {
        var data = cardBytes;
        if (settings.EncryptionType == 1) data = AesDecryptEs3(data, settings.Password);
        if (settings.CompressionType == 1) data = GzipDecompress(data);
        return data;
    }

    private static byte[] AesDecryptEs3(byte[] cardBytes, string password)
    {
        if (cardBytes.Length < 32) throw new InvalidOperationException("암호화된 .card 파일이 너무 짧습니다.");
        var iv = cardBytes[..16];
        var ciphertext = cardBytes[16..];

        using var aes = Aes.Create();
        aes.KeySize = 128;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = DeriveKey(password, iv);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
    }

    private static byte[] AesEncryptEs3(byte[] plainBytes, string password)
    {
        var iv = RandomNumberGenerator.GetBytes(16);
        using var aes = Aes.Create();
        aes.KeySize = 128;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = DeriveKey(password, iv);
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor();
        var cipher = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return iv.Concat(cipher).ToArray();
    }

    private static byte[] DeriveKey(string password, byte[] iv)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), iv, 100, HashAlgorithmName.SHA1);
        return pbkdf2.GetBytes(16);
    }

    private static byte[] GzipDecompress(byte[] data)
    {
        using var input = new MemoryStream(data);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }

    private static byte[] GzipCompress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            gzip.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }
}

internal static class CardJson
{
    public static IEnumerable<KeyValuePair<string, string>> GetEditableTextFields(JsonObject obj)
    {
        foreach (var pair in obj)
        {
            if (pair.Key is "bgmTitle" or "flavorText" or "tags") continue;
            if (pair.Value is not JsonObject entry) continue;
            if (!entry.TryGetPropertyValue("value", out var valueNode)) continue;
            if (LooksLikeByteEntry(entry)) continue;
            if (valueNode is JsonValue value && value.TryGetValue<string>(out var text))
                yield return new KeyValuePair<string, string>(pair.Key, text);
        }
    }

    public static string GetStringValue(JsonObject obj, string key)
    {
        if (obj[key] is JsonObject entry
            && entry.TryGetPropertyValue("value", out var valueNode)
            && valueNode is JsonValue value
            && value.TryGetValue<string>(out var text))
        {
            return text;
        }
        return "";
    }

    public static string GetValueText(JsonObject obj, string key)
    {
        if (obj[key] is not JsonObject entry || !entry.TryGetPropertyValue("value", out var valueNode) || valueNode is null)
            return "";
        if (valueNode is JsonValue value)
        {
            if (value.TryGetValue<string>(out var text)) return text;
            return valueNode.ToJsonString();
        }
        return valueNode.ToJsonString();
    }

    public static List<string> GetStringList(JsonObject obj, string key)
    {
        var result = new List<string>();
        if (obj[key] is not JsonObject entry) return result;
        if (!entry.TryGetPropertyValue("value", out var valueNode) || valueNode is null) return result;

        if (valueNode is JsonArray array)
        {
            foreach (var item in array)
            {
                if (item is JsonValue value && value.TryGetValue<string>(out var text))
                    result.Add(text);
                else if (item is not null)
                    result.Add(item.ToString());
            }
            return result;
        }

        if (valueNode is JsonValue singleValue && singleValue.TryGetValue<string>(out var singleText))
        {
            result.Add(singleText);
        }

        return result;
    }

    public static string[] GetTagValues(JsonObject obj, string key)
    {
        if (obj[key] is not JsonObject entry
            || !entry.TryGetPropertyValue("value", out var valueNode)
            || valueNode is null)
        {
            return ["", "", ""];
        }

        var tags = new List<string>();
        if (valueNode is JsonArray array)
        {
            foreach (var item in array)
            {
                if (item is JsonValue value && value.TryGetValue<string>(out var text))
                    tags.Add(text);
                else
                    tags.Add(item?.ToString() ?? "");
            }
        }
        else if (valueNode is JsonValue value && value.TryGetValue<string>(out var singleText))
        {
            tags.Add(singleText);
        }

        while (tags.Count < 3)
            tags.Add("");
        return tags.Take(3).ToArray();
    }

    public static void SetTagValues(JsonObject obj, string key, IReadOnlyList<string> tags)
    {
        if (obj[key] is not JsonObject entry)
        {
            entry = new JsonObject();
            obj[key] = entry;
        }

        var array = new JsonArray();
        for (var i = 0; i < 3; i++)
            array.Add(i < tags.Count ? tags[i].Trim() : "");
        entry["value"] = array;
    }

    public static bool HasStringValue(JsonObject obj, string key)
    {
        return obj[key] is JsonObject entry
            && entry.TryGetPropertyValue("value", out var valueNode)
            && valueNode is JsonValue value
            && value.TryGetValue<string>(out _);
    }

    public static void SetStringValue(JsonObject obj, string key, string value)
    {
        if (obj[key] is not JsonObject entry)
        {
            entry = new JsonObject { ["__type"] = "string" };
            obj[key] = entry;
        }
        entry["value"] = value;
    }

    public static bool TryGetBytes(JsonObject obj, string key, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        if (obj[key] is not JsonObject entry || !LooksLikeByteEntry(entry)) return false;
        var base64 = entry["value"]?.GetValue<string>() ?? "";
        if (string.IsNullOrWhiteSpace(base64)) return false;
        try
        {
            bytes = Convert.FromBase64String(base64);
            return true;
        }
        catch (FormatException)
        {
            bytes = Array.Empty<byte>();
            return false;
        }
    }

    public static void SetBytes(JsonObject obj, string key, byte[] bytes)
    {
        if (obj[key] is not JsonObject entry || !LooksLikeByteEntry(entry))
        {
            entry = new JsonObject { ["__type"] = "System.Byte[],mscorlib" };
            obj[key] = entry;
        }
        entry["value"] = Convert.ToBase64String(bytes);
    }

    private static bool LooksLikeByteEntry(JsonObject entry)
    {
        if (!entry.TryGetPropertyValue("__type", out var typeNode)) return false;
        var type = typeNode?.ToString() ?? "";
        return type == "22" || type.StartsWith("System.Byte[]", StringComparison.Ordinal);
    }
}

