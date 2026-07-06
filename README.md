# Monster-Cards-Editor_KZ.exe KOR/JPN/ENG ReadME.md


=======================================================
이 툴은 Gasai Games 사의 Monster Cards 게임의 카드 Editor 입니다.
* 주의 : 이 툴은 인게임의 밸런스를 헤치는 Edit가 불가합니다. [공격, 종족, 속성 등]

다운로드 방법 : 초록색 Code를 누른 후, Donwload ZIP을 눌러 파일을 받고 MonsterCardExeEditor.exe 를 실행.
=======================================================


MonsterCards .card 파일의 텍스트, 이미지, BGM, JSON을 수정·저장하고 레이어 미리보기와 후처리를 지원하는 편집 도구입니다.
본 게임의 리롤에 관련해 방해하고 싶은 생각은 없으므로 카드의 공격력, 종족, 스킬 등은 수정할 수 없게 하였습니다.
DLC 기능은 본 게임의 유료 콘텐츠이므로 함부로 건드리지 않는 것으로 했기에 수정 불가합니다. 카드를 생성할 때 지정해주세요.

[주요 기능]
- MonsterCards .card 파일을 열어 내부 텍스트, 이미지 bytes, BGM bytes, 고급 JSON을 확인하고 수정할 수 있습니다.
- ES3Defaults.asset을 선택해 게임 저장 설정을 참고할 수 있으며, 기본 비밀번호 설정도 지원합니다.
- 카드 파일명, 카드 이름, 주요 텍스트 필드, 플레이버 텍스트를 편집할 수 있습니다.
- 공격력, 종족, 스킬, DLC 관련 값은 게임 플레이 밸런스와 유료 콘텐츠 보호를 위해 수정 대상에서 제외했습니다.
- 플레이버 텍스트 자동 출력 기능을 지원합니다.
  형식: RACE / ELE / DESC / Owner
  Owner 값은 creatorName 필드를 우선 사용합니다.
- 특수 링크 탭에서 카드의 연결 대상 정보를 확인할 수 있습니다.
- 카드 목록 필터를 지원합니다.
  레어도 순 정렬, ㄱ~ㄴ / A~Z / 일본어 순 정렬을 켜고 끌 수 있습니다.
- image1Bytes, image2Bytes, image3Bytes를 교체하거나 추출할 수 있습니다.
- BGM bytes를 교체하거나 추출할 수 있습니다.
- 교체한 이미지는 원본 PNG bytes를 기준으로 처리하며, 기본 상태에서는 불필요하게 재인코딩하지 않습니다.
- 이미지 1/2/3 공통 조절 기능을 지원합니다.
  DOT 스타일: 512, 258, 128, 64, 32, 16
  색수차, Hue, Saturation, 중앙 기준 확대/축소
- 각 이미지 레이어마다 후처리 1/2/3을 따로 설정할 수 있습니다.
  Lens Flare, Bloom, Depth of Field, Blur, Film Grain, Noise, Vignette, Contrast,
  Monochrome, CRT Scanlines, Pixelation, Halftone, Glitch, RGB Split, TV Static, Interlace
- 이미지 미리보기 팝업을 지원합니다.
  image3은 배경, image2는 중간, image1은 가장 위 레이어로 합성됩니다.
- 카드 프레임 미리보기 토글을 지원합니다.
  그림 표시 영역은 그대로 보이고, 바깥 프레임 영역은 회색 80% 오버레이로 표시됩니다.
- 고급 JSON 탭에서 전체 JSON을 확인하고 적용할 수 있습니다.
- 저장 시 .card 파일을 다시 암호화/압축하고, 저장 직후 복호화/압축해제/JSON 검증을 수행합니다.
- 카드 저장 후 확대/축소 값은 자동으로 100%로 되돌아갑니다.

[사용 순서]
1. 인게임에서 카드를 생성합니다.
2. 카드 이름을 적고 그림을 백지 상태로 놓은 뒤 저장합니다.
3. 에디터를 실행하고 카드 폴더를 선택하거나 기본 폴더를 사용합니다.
4. 필요하면 ES3Defaults.asset을 선택합니다.
5. 카드 목록에서 편집할 .card 파일을 선택합니다.
6. 이미지, 설명, BGM, 후처리, JSON을 수정합니다.
7. 카드 저장 버튼을 눌러 .card 파일을 저장합니다.

[주의]
- 사용 전 원본 .card 파일을 백업하세요.
- 공격력, 종족, 스킬, DLC 기능은 이 에디터에서 수정할 수 없습니다.
- .Json 수정 시 발생하는 불이익에 대해서는 책임지지 않습니다.
- 타인에게 불쾌감을 줄 수 있는 행위, 공정함을 깨트리는 행위는 우리 모두 삼가합시다.
- 비공식 편집 도구이므로 게임 업데이트로 저장 형식이 바뀌면 동작하지 않을 수 있습니다.
- 본 도구 사용으로 발생하는 불이익에 대해 제작자는 책임지지 않습니다.

--------------------------------------------------------------

MonsterCards の .card ファイル内のテキスト、画像、BGM、JSONを編集・保存し、レイヤープレビューと画像後処理を利用できる編集ツールです。
本ツールはゲーム内のリロール要素を妨げる目的ではないため、カードの攻撃力、種族、スキルなどのゲームプレイ値は編集できないようにしています。
DLC機能は本ゲームの有料コンテンツであるため、勝手に変更しない方針として編集不可にしています。カード生成時にゲーム内で指定してください。

[主な機能]
- MonsterCards の .card ファイルを開き、内部テキスト、画像 bytes、BGM bytes、高度な JSON を確認・編集できます。
- ES3Defaults.asset を選択してゲーム保存設定を参照できます。既定パスワード設定にも対応しています。
- カードファイル名、カード名、主要テキスト項目、フレーバーテキストを編集できます。
- 攻撃力、種族、スキル、DLC関連値はゲームバランスと有料コンテンツ保護のため編集対象から除外しています。
- フレーバーテキストの自動出力に対応しています。
  形式: RACE / ELE / DESC / Owner
  Owner は creatorName フィールドを優先して使用します。
- 特殊リンクタブでカードのリンク先情報を確認できます。
- カード一覧フィルターに対応しています。
  レア度順、韓国語/英語/日本語名順の並び替えをオン/オフできます。
- image1Bytes、image2Bytes、image3Bytes の置換と抽出ができます。
- BGM bytes の置換と抽出ができます。
- 置換した画像は元の PNG bytes を基準に扱います。編集がない場合、不要な再エンコードを行わず元 bytes を保持します。
- image1/2/3 共通画像調整に対応しています。
  DOT スタイル: 512, 258, 128, 64, 32, 16
  色収差、Hue、Saturation、中央基準の拡大/縮小
- 各画像レイヤーごとに後処理1/2/3を個別設定できます。
  Lens Flare, Bloom, Depth of Field, Blur, Film Grain, Noise, Vignette, Contrast,
  Monochrome, CRT Scanlines, Pixelation, Halftone, Glitch, RGB Split, TV Static, Interlace
- 画像プレビューのポップアップに対応しています。
  image3 が背景、image2 が中間、image1 が最前面レイヤーとして合成されます。
- カードフレームプレビューの切り替えに対応しています。
  画像表示領域はそのまま見え、外側のフレーム領域は80%のグレーオーバーレイで表示されます。
- 高度な JSON タブで全体 JSON を確認し、適用できます。
- 保存時に .card ファイルを再暗号化/再圧縮し、復号/展開/JSON解析で検証します。
- カード保存後、拡大/縮小の値は自動的に100%へ戻ります。

[基本的な使い方]
1. ゲーム内でカードを作成します。
2. カード名を入力し、画像は白紙の状態で保存します。
3. エディターを起動し、カードフォルダーを選択するか既定フォルダーを使用します。
4. 必要に応じて ES3Defaults.asset を選択します。
5. カード一覧から編集する .card ファイルを選択します。
6. 画像、説明、BGM、後処理、JSONを編集します。
7. カード保存ボタンを押して .card ファイルを書き込みます。

[注意]
- 編集前に必ず元の .card ファイルをバックアップしてください。
- 攻撃力、種族、スキル、DLC機能は本エディターでは編集できません。
- .Json の編集によって発生した不利益について、製作者は責任を負いません。
- 他人に不快感を与える行為や、公平性を損なう行為は、みんなで控えましょう。
- 本ツールは非公式編集ツールです。ゲーム更新により保存形式が変わると動作しない場合があります。
- 本ツールの使用によって発生した不利益について、製作者は責任を負いません。


-------------------------------------------------------

This editor lets you modify and save MonsterCards .card text, images, BGM, and JSON, with layer previews and image post-processing tools.
This tool is not intended to interfere with the game's reroll system, so card attack power, race, skills, and similar gameplay values cannot be edited.
DLC features are paid game content and are intentionally not editable. Please set DLC-related options when creating the card in-game.

[Main Features]
- Open MonsterCards .card files and inspect or edit internal text, image bytes, BGM bytes, and advanced JSON.
- Select ES3Defaults.asset to reference game save settings. The default password setting is also supported.
- Edit card file name, card name, main text fields, and flavor text.
- Attack power, race, skills, and DLC-related values are excluded from editing to respect game balance and paid content.
- Generate flavor text automatically.
  Format: RACE / ELE / DESC / Owner
  Owner uses the creatorName field first.
- View linked target information in the Special Link tab.
- Filter the card list.
  Toggle rarity sorting or name sorting for Korean, English, and Japanese text.
- Replace or export image1Bytes, image2Bytes, and image3Bytes.
- Replace or export BGM bytes.
- Replaced images are handled from the original PNG bytes. When no edits are active, the original bytes are preserved without unnecessary re-encoding.
- Shared image controls are available for image layers 1/2/3.
  DOT styles: 512, 258, 128, 64, 32, 16
  Chromatic shift, Hue, Saturation, center-based Scale
- Each image layer can have its own Post 1 / Post 2 / Post 3 effects.
  Lens Flare, Bloom, Depth of Field, Blur, Film Grain, Noise, Vignette, Contrast,
  Monochrome, CRT Scanlines, Pixelation, Halftone, Glitch, RGB Split, TV Static, Interlace
- Image preview popup is supported.
  image3 is the background, image2 is the middle layer, and image1 is the top layer.
- Card frame preview toggle is supported.
  The image display area remains visible, while the outer frame area is shown with an 80% gray overlay.
- View and apply the full JSON in the Advanced JSON tab.
- On save, the .card file is encrypted/compressed again and verified by decrypt/decompress/JSON parse.
- After saving a card, the Scale value automatically returns to 100%.

[Basic Usage]
1. Create a card in-game.
2. Enter the card name, leave the artwork blank, and save it.
3. Launch the editor and select the card folder or use the default folder.
4. Select ES3Defaults.asset if needed.
5. Choose the .card file from the card list.
6. Edit images, descriptions, BGM, post-processing, or JSON.
7. Press Save Card to write the .card file.

[Notice]
- Always back up the original .card file before editing.
- Attack power, race, skills, and DLC features cannot be edited with this tool.
- The creator is not responsible for any disadvantage caused by editing .Json data.
- Please avoid actions that may offend others or break fairness for everyone.
- This is an unofficial editor. It may stop working if a game update changes the save format.
- The creator is not responsible for any disadvantage caused by using this tool.
