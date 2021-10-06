using Newtonsoft.Json;

namespace MyAuth.OAuthPoint.Models
{
    /// <summary>
    /// This scope value requests access to the End-User's default profile Claims
    /// </summary>
    public class ProfileScopeClaims : ScopeClaims
    {
        /// <summary>
        /// End-User's full name in displayable form including all name parts, possibly including titles and suffixes, ordered according to the End-User's locale and preferences.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Surname(s) or last name(s) of the End-User. Note that in some cultures, people can have multiple family names or no family name; all can be present, with the names being separated by space characters.
        /// </summary>
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }
        /// <summary>
        /// Given name(s) or first name(s) of the End-User. Note that in some cultures, people can have multiple given names; all can be present, with the names being separated by space characters.
        /// </summary>
        [JsonProperty("given_name")]
        public string GivenName { get; set; }
        /// <summary>
        /// Middle name(s) of the End-User. Note that in some cultures, people can have multiple middle names; all can be present, with the names being separated by space characters. Also note that in some cultures, middle names are not used.
        /// </summary>
        [JsonProperty("middle_name")]
        public string MiddleName { get; set; }
        /// <summary>
        /// Casual name of the End-User that may or may not be the same as the given_name. For instance, a nickname value of Mike might be returned alongside a given_name value of Michael.
        /// </summary>
        [JsonProperty("nickname")]
        public string NickName { get; set; }
        /// <summary>
        /// Shorthand name by which the End-User wishes to be referred to at the RP, such as janedoe or j.doe. This value MAY be any valid JSON string including special characters such as @, /, or whitespace. The RP MUST NOT rely upon this value being unique, as discussed in Section 5.7.
        /// </summary>
        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }
        /// <summary>
        /// URL of the End-User's profile page. The contents of this Web page SHOULD be about the End-User.
        /// </summary>
        [JsonProperty("profile")]
        public string Profile { get; set; }
        /// <summary>
        /// URL of the End-User's profile picture. This URL MUST refer to an image file (for example, a PNG, JPEG, or GIF image file), rather than to a Web page containing an image. Note that this URL SHOULD specifically reference a profile photo of the End-User suitable for displaying when describing the End-User, rather than an arbitrary photo taken by the End-User.
        /// </summary>
        [JsonProperty("picture")]
        public string Picture { get; set; }
        /// <summary>
        /// URL of the End-User's Web page or blog. This Web page SHOULD contain information published by the End-User or an organization that the End-User is affiliated with.
        /// </summary>
        [JsonProperty("website")]
        public string Website { get; set; }
        /// <summary>
        /// End-User's gender. Values defined by this specification are female and male. Other values MAY be used when neither of the defined values are applicable.
        /// </summary>
        [JsonProperty("gender")]
        public string Gender { get; set; }
        /// <summary>
        /// End-User's birthday, represented as an ISO 8601:2004 [ISO8601‑2004] YYYY-MM-DD format. The year MAY be 0000, indicating that it is omitted. To represent only the year, YYYY format is allowed. Note that depending on the underlying platform's date related function, providing just year can result in varying month and day, so the implementers need to take this factor into account to correctly process the dates.
        /// </summary>
        [JsonProperty("birthdate")]
        public string Birthdate { get; set; }
        /// <summary>
        /// String from zoneinfo [zoneinfo] time zone database representing the End-User's time zone. For example, Europe/Paris or America/Los_Angeles.
        /// </summary>
        [JsonProperty("zoneinfo")]
        public string Zoneinfo { get; set; }
        /// <summary>
        /// End-User's locale, represented as a BCP47 [RFC5646] language tag. This is typically an ISO 639-1 Alpha-2 [ISO639‑1] language code in lowercase and an ISO 3166-1 Alpha-2 [ISO3166‑1] country code in uppercase, separated by a dash. For example, en-US or fr-CA. As a compatibility note, some implementations have used an underscore as the separator rather than a dash, for example, en_US; Relying Parties MAY choose to accept this locale syntax as well.
        /// </summary>
        [JsonProperty("locale")]
        public string Locate { get; set; }
        /// <summary>
        /// Time the End-User's information was last updated. Its value is a JSON number representing the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until the date/time.
        /// </summary>
        [JsonProperty("updated_at")]
        public string UpdatedAt{ get; set; }

    }
}