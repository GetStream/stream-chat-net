using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Language
    {
        None,

        [EnumMember(Value = "af")]
        AF,

        [EnumMember(Value = "am")]
        AM,

        [EnumMember(Value = "ar")]
        AR,

        [EnumMember(Value = "az")]
        AZ,

        [EnumMember(Value = "bg")]
        BG,

        [EnumMember(Value = "bn")]
        BN,

        [EnumMember(Value = "bs")]
        BS,

        [EnumMember(Value = "cs")]
        CS,

        [EnumMember(Value = "da")]
        DA,

        [EnumMember(Value = "de")]
        DE,

        [EnumMember(Value = "el")]
        EL,

        [EnumMember(Value = "en")]
        EN,

        [EnumMember(Value = "es")]
        ES,

        [EnumMember(Value = "es-MX")]
        ES_MX,

        [EnumMember(Value = "et")]
        ET,

        [EnumMember(Value = "fa")]
        FA,

        [EnumMember(Value = "fa-AF")]
        FA_AF,

        [EnumMember(Value = "fi")]
        FI,

        [EnumMember(Value = "fr")]
        FR,

        [EnumMember(Value = "fr-CA")]
        FR_CA,

        [EnumMember(Value = "ha")]
        HA,

        [EnumMember(Value = "he")]
        HE,

        [EnumMember(Value = "hi")]
        HI,

        [EnumMember(Value = "hr")]
        HR,

        [EnumMember(Value = "hu")]
        HU,

        [EnumMember(Value = "id")]
        ID,

        [EnumMember(Value = "it")]
        IT,

        [EnumMember(Value = "ja")]
        JA,

        [EnumMember(Value = "ka")]
        KA,

        [EnumMember(Value = "ko")]
        KO,

        [EnumMember(Value = "lv")]
        LV,

        [EnumMember(Value = "ms")]
        MS,

        [EnumMember(Value = "nl")]
        NL,

        [EnumMember(Value = "no")]
        NO,

        [EnumMember(Value = "pl")]
        PL,

        [EnumMember(Value = "ps")]
        PS,

        [EnumMember(Value = "pt")]
        PT,

        [EnumMember(Value = "ro")]
        RO,

        [EnumMember(Value = "ru")]
        RU,

        [EnumMember(Value = "sk")]
        SK,

        [EnumMember(Value = "sl")]
        SL,

        [EnumMember(Value = "so")]
        SO,

        [EnumMember(Value = "sq")]
        SQ,

        [EnumMember(Value = "sr")]
        SR,

        [EnumMember(Value = "sv")]
        SV,

        [EnumMember(Value = "sw")]
        SW,

        [EnumMember(Value = "ta")]
        TA,

        [EnumMember(Value = "th")]
        TH,

        [EnumMember(Value = "tl")]
        TL,

        [EnumMember(Value = "tr")]
        TR,

        [EnumMember(Value = "uk")]
        UK,

        [EnumMember(Value = "ur")]
        UR,

        [EnumMember(Value = "vi")]
        VI,

        [EnumMember(Value = "zh")]
        ZH,

        [EnumMember(Value = "zh-TW")]
        ZH_TW,
    }
}