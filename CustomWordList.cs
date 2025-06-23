using System.Collections.Generic;

namespace GHMSRestaurantBot
{
    /// <summary>
    /// Contains a custom list of food-related words for spell checking.
    /// </summary>
    public static class CustomWordList
    {
        /// <summary>
        /// List of custom words related to food and restaurant terms.
        /// </summary>
        public static List<string> Words => new List<string>
        {
            "achari", "adai", "aloo", "andhra", "bajji", "balchao", "batata", "benne", "bhara", "bhel", "bhuna",
            "bhutta", "bonjour", "boti", "bournvita", "chaat", "chai", "chana", "chettinad", "chilli", "chingri", "corn",
            "dahi", "dal", "dhokla", "dosa", "dosas", "fanta", "galouti", "gassi", "goan", "gobi", "gosht", "hara",
            "hariyali", "hola", "horlicks", "hyderabadi", "idli", "idlies", "indian", "jain", "jhinga", "kakori",
            "kanchipuram", "kerala", "khaman", "kodi", "kolhapuri", "koliwada", "konju", "kosha", "kura", "kuzhambu",
            "laal", "maas", "malabar", "malai", "mallige", "manchurian", "mangsho", "masala", "medu", "mirchi",
            "moilee", "moong", "mudda", "mysore", "namaskar", "namaste", "neer", "nonvegiterian", "pakora", "paneer",
            "pani", "papad", "papdi", "patia", "pav", "pesarattu", "podi", "poha", "pollichathu", "puri", "pyaza",
            "ragi", "rava", "roasted", "rogan", "royallu", "saag", "sabudana", "sada", "samabar", "sambar", "sanna",
            "schezwan", "seekh", "sev", "shami", "sukka", "tata", "thatte", "tikka", "tikki", "uttappam", "vada",
            "varutharaccha", "vegiterian", "vepudu", "vindaloo", "zup"
        };
    }
}