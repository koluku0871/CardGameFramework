using System.Collections.Generic;

public class ConstManager
{
    public const string DIRECTORY_PATH_TO_DECK = "/deck/";
    public const string DIRECTORY_PATH_TO_OPTION = "/option/";
    public const string DIRECTORY_PATH_TO_BUNDLES = "/bundles/";
    public const string DIRECTORY_PATH_TO_EXE = "/exe/";

#if UNITY_EDITOR
    public static readonly string DIRECTORY_PATH = "E:/UnityProject/CardGameFramework/data";
#else
    public static readonly string DIRECTORY_PATH = UnityEngine.Application.dataPath;
#endif
    public static readonly string DIRECTORY_FULL_PATH_TO_DECK = DIRECTORY_PATH + DIRECTORY_PATH_TO_DECK;
    public static readonly string DIRECTORY_FULL_PATH_TO_OPTION = DIRECTORY_PATH + DIRECTORY_PATH_TO_OPTION;
    public static readonly string DIRECTORY_FULL_PATH_TO_BUNDLES = DIRECTORY_PATH + DIRECTORY_PATH_TO_BUNDLES;
    public static readonly string DIRECTORY_FULL_PATH_TO_EXE = DIRECTORY_PATH + DIRECTORY_PATH_TO_EXE;

    public enum PhotonObjectType
    {
        NONE = 0,
        CARD,
        SOULCORE,
        CORE,
        MARK,
        DAMAGE,
    }

    public enum CorePosType
    {
        NONE = 0,
        LIFE,
        RESERVE,
        FIELD,
        TRASH,
        EXCLUSION,
    }

    // https://www.battlespirits.com/rule/limited.php
    // 禁止カード
    public static readonly List<string> DECK_CARD_NG_LIST = new List<string>()
    {
        // ステゴウロ
        "BS60-001",
        // 雲数ノ絆神フラグジャッジメント
        "BS64-X02",
        // 秘密の訓練場
        "BS66-068",
        // 冥府大魔導エシュゾ
        "BS52-017",
        // ダークイニシエーション
        "BS32-074",

        // 七大英雄獣ヘクトル
        "BS45-024",
        // バインドエッジ
        "BS56-075",
        // スワロウテイル
        "BS56-RV006",
        // スワロウテイル
        "BS28-082",
        // ヴァンピーアヴォルク
        "BS57-011",

        // バタフライジャマー
        "BS57-078",
        // セクシーバズーカ
        "BS57-083",
        // 月鬼城
        "BS61-062",
        // 宇宙世紀憲章
        "SD54-015",
        // 赤魔神
        "BS39-049",

        // 紫魔神
        "BS39-051",
        // 緑魔神
        "BS39-052",
        // 白魔神
        "BS39-055",
        // 黄魔神
        "BS39-057",
        // 青魔神
        "BS39-059",

        // 吸血伯爵エル・サルバトール
        "BS52-019",
        // 三災獣・海災ディザイアタン
        "BS53-XX02",
        // 滅神星龍ダークヴルム・ノヴァ
        "SD41-RV X01",
        // 滅神星龍ダークヴルム・ノヴァ
        "BS11-X02",
        // 時冠超神シン・クロノス
        "BS59-X07",

        // 闇輝石六将 機械獣神フェンリグ
        "BS43-X04",
        // グラナート・ゴレム
        "BS52-058",
        // 醒龍皇リバーサルドラゴン
        "BS52-X01",
        // 醒海皇ヴァルシャルク
        "BS52-X06",
        // 蛇皇龍ガルメジャード
        "BS54-X03",

        // 黄泉ノ皇蛇ライウンオロチ
        "BS56-021",
        // 獄土の四魔卿マグナマイザー
        "BS38-X02",
        // やっぱ拾ったカードは弱い
        "BSC32-031",
        // 冥犬ケルル・ベロス
        "BS02-063",
        // ヤシウム
        "BS38-RV003",

        // ヤシウム
        "BS10-006",
        // 断罪ノ滅刃ジャッジメント・ドラゴン・ソード
        "BS44-10thX01",
        // 龍皇海賊団 見張りのネコジャジャ
        "BS41-063",
        // 導化姫トリックスター
        "BS12-039",
        // ネイチャーフォース
        "BS02-097",

        // グレートリンク
        "BS04-089",
        // インフェルノアイズ
        "BS04-096",
        // 烈の覇王セイリュービ
        "BS16-X03",
        // ウィッグバインド
        "BS11-082",
        // ハンドタイフーン
        "BS10-111",

        // トリックプランク
        "BS04-105",
        // 巨人港
        "BS13-071",
        // ルナティックシール
        "BS10-108",
        // 魔法監視塔
        "SD02-014",
        // ストームドロー
        "BSC22-CP02",

        // ストームドロー
        "BS01-132",
        // ライフチェイン
        "BS02-099",
        // 栄光の表彰台
        "BS04-088",
        // イビルオーラ
        "BSC22-CP01",
        // イビルオーラ
        "BS01-124",
    };

    // 一つのデッキに何枚でも入れられるカード
    public static readonly List<string> DECK_CARD_DUPLICATE_OK_LIST = new List<string>()
    {
        /**
         * BS
         */

        // 四甲天カメジュウジ
        "BS28-020",
        // コツノアントマン
        "BS40-023",
        // 華兵・ジー
        "BS68-024",
        // デストロイア（幼体）
        "BSC19-001",
        // 宇宙忍者バルタン星人ベーシカルバージョン
        "BSC24-024",

        // 超古代怨霊翼獣シビトゾイガー
        "CB01-032",
        // ディアボロモン
        "CB02-060",
        // デジタマ
        "CB02-061",
        // マスカレイド・ドーパント
        "CB04-014",
        // クラモン
        "CB05-053",

        // 仮面ライダーオーズ ガタキリバ コンボ
        "CB08-061",
        // セルメダル
        "CB08-077",
        // ヒューマギア
        "CB12-017",
        // 宇宙凶険怪獣ケルビム［ウルトラ怪獣2020］
        "CB18-003",
        // EVANGELION Mark.07
        "CB23-018",

        // 仮面ライダーアルティメットバイス
        "CB24-X03",
        // 仮面ライダーアルティメットリバイ
        "CB24-X05",
        // ジェガン[U.C.0093]
        "CB25-002",
        // ガイガンミレース
        "CB28-002",
        // デストロイア（幼体）
        "CB28-RV001",

        // ガンドノード
        "CB29-020",
        // ELS
        "CB29-038",
        // ミニーズ特攻隊
        "CP13-01",
        // ミニーズ大運動会
        "P15-17",
        // BooBooマン
        //"P16-26",

        // ジェガン
        "SD54-004",

        /**
         * デジモン
         */

        // エオスモン
        "BT6-085",
        "BT6-085_P1",
        // ベムモン
        "BT11-061",
        "BT11-061_P1",
        "BT11-061_P2",
        // ADR-02=サーチャー
        "EX2-046",
        "EX2-046_P1",
        // アイズモン：スキャッターモード
        "EX9-048",
        "EX9-048_P1",
        // イーター（原種形態）
        "BT22-079",

        /**
         * ホロライブ
         */

        // 白エール
        "hY01-001_C",
        // 緑エール
        "hY02-001_C",
        // 赤エール
        "hY03-001_C",
        // 青エール
        "hY04-001_C",
        // 紫エール
        "hY05-001_C",

    };

    // 制限カード(1枚まで)
    public static readonly List<string> DECK_CARD_DUPLICATE_NG_LIST = new List<string>()
    {
        // ゴッドブレイク
        "BS52-X09",
        // 創醒の大創界石 / 大地の御子ヴィーナ
        "BS55-TX04",
        // 魔卿執事バランドール
        "BS64-059",
        // 陰陽童
        "BS52-RV002",
        // 陰陽童
        "BS32-011",

        // ドローンアント
        "BS56-022",
        // スカラベビートルドローン
        "BS56-025",
        // イノレーサー
        "BS60-063",
        // ヴィルカイックビーチ
        "BS60-083",
        // 魔界大鎌ベルゼビートサイズ
        "LM19-02",
        
        // ゴッドシーカー 超星使徒タルボス
        "SD51-005",
        // 道化神メルト
        "BS41-XX01",
        // 龍皇海賊団 副船長アオザック
        "BS42-069",
        // 天空の光剣クラウン・ソーラー
        "BS51-CP04",
        // 天空の光剣クラウン・ソーラー
        "BS21-X07",
        
        // 赤の世界 / 赤き神龍皇
        "BS52-TX01",
        // エンシェントドラゴン・フェブラーニ
        "BS52-010",
        // 天空の光剣クラウン・ソーラーX / 天空の光剣クラウン・ソーラーX -転醒化身-
        "BS58-TCP02",
        // 蠱惑姫ミズア
        "BS60-051",
        // アントラーウミウシ
        "BS60-064",
        
        // 時空龍クロノ・ドラゴン / 時空龍皇クロノバース・ドラグーン
        "SD55-TX01",
        // 大天使ミカファール
        "BS02-X08",
        // 宇宙海賊船ボーンシャーク号 / 宇宙海賊船ボーンシャーク号 -襲撃形態-
        "BS56-072",
        // 星霊黄龍ファンロン
        "BS57-X07",
        // 滅神星龍ダークヴルム・ノヴァX
        "SD51-X02",
        
        // 翼神機グラン・ウォーデン
        "BS08-X32",
        // 翼神機グラン・ウォーデン
        "BS43-RV X06",
        // 小氷姫クラーラ
        "BS52-032",
        // 超覇王ロード・ドラゴン・零
        "BS55-X02",
        // プトレマイオス
        "SD53-014",
        
        // オワリノ世界 / 天魔王ゴッド・ゼクス　-焉ノ型-
        "SD57-006",
        // インペリアルドラモン パラディンモード
        "CB05-XX01",
        // アルケーガンダム
        "CB13-X05",
        // 魔導氷姫アガーフィア
        "BS55-047",
        // 新しき世界 / 風雅龍エレア・ラグーン
        "SD55-011",
        
        // ト音獣ジークレフキャット
        "BS51-042",
        // 歴戦騎士ドルク・エヴィデンス
        "BS50-X03",
        // 超星使徒コーディリア
        "SD51-004",
        // 煌星竜スピキュールドラゴン
        "BS42-007",
        // サンピラー・ドラゴン
        "SD10-009",
        
        // 吊られた古城
        "BS15-063",
        // 動かざる山の本陣
        "BSC05-018",
        // 維持神龍トリヴィ・クラマ
        "BS50-X04",
        // 十式戦鬼・断蔵
        "BS50-015",
        // 五線獣バッファロースコア
        "BS51-038",
        
        // 甲の使徒レーディア
        "BS35-005",
        // 太陽神獣セクメトゥーム
        "BS46-X05",
        // シアーハートアタック
        "BS46-095",
        // ムリダンガムドラゴン
        "BS48-025",
        // 太陽の守護蟲ケプリ
        "BS46-056",
        
        // 龍星の射手リュキオース
        "BS45-X01",
        // デス・ヘイズ
        "BS12-052",
        // 幻魔神
        "BS39-054",
        // 伝説王者タイタス・エル・グランデ
        "BS40-X06",
        // ブレイドラ
        "SD43-RV001",
        
        // ブレイドラ
        "SD03-001",
        // アイツのカード
        "BSC29-007",
        // 一月幼神ディアヌス・キッズ
        "LM17-01",
        // 未の十二神皇グロリアス・シープ
        "BS35-X04",
        // 秩序龍機νジークフリード
        "SD39-X01",
        
        // マントラドロー
        "BS31-112",
        // 申の十二神皇ハヌマーリン
        "BS37-X05",
        // 果物女王マンゴスティナ
        "BS39-036",
        // 庚の猿王ヴァーリン
        "BS39-039",
        // 侵食されゆく尖塔
        "BS06-084",
        
        // アルティメット・ダ・ゴン
        "PX14-06",
        // フォビドゥングレイヴ
        "BS26-075",
        // 鎧闘鬼ラショウ
        "BS31-017",
        // 五聖童子
        "BS32-048",
        // 紫煙獅子
        "BS33-019",
        
        // 海底に眠りし古代都市
        "BS08-066",
        // アトライア・ハイドラ
        "BS30-035",
        // ネクロブライト
        "BS30-075",
        // 闇騎士トリスタン
        "P073",
        // 血塗られた魔具
        "BS38-RV028",
        
        // 血塗られた魔具
        "BS13-063",
        // 牙皇ケルベロード
        "SD03-012",
        // 侵されざる聖域
        "BS04-082",
        // 大天使ヴァリエル
        "BS03-X11",
        // マインドコントロール
        "BS02-093",
    };

    // 制限カード(5枚まで)
    public static readonly List<string> DECK_CARD_DUPLICATE_5_NG_LIST = new List<string>()
    {
        // ドードーマギア
        "CB15-012",
        // ベギルペンデ
        "CB27-007",
    };

    // 制限カード(20枚まで)
    public static readonly List<string> DECK_CARD_DUPLICATE_20_NG_LIST = new List<string>()
    {
        // 子フィンクス
        "BS46-058",
        // 連召喚：子フィンクスフィールド
        "BS67-085",
        // デ・リーパーADR-02 サーチャー
        "CB07-043",
        // BooBooマン
        "P16-26",
    };

    // 制限カード(29枚まで)
    public static readonly List<string> DECK_CARD_DUPLICATE_29_NG_LIST = new List<string>()
    {
        // ジンクス
        "CB13-048",
    };

    // 一つのデッキに入れられる同カード枚数
    public const int DECK_CARD_DUPLICATE_COUNT = 4;

    public const int DECK_CARD_MIN_COUNT = 40;
}
