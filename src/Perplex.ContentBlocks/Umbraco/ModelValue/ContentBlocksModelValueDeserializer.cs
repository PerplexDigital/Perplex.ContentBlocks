using Newtonsoft.Json;

namespace Perplex.ContentBlocks.Umbraco.ModelValue
{
    public class ContentBlocksModelValueDeserializer
    {
        /// <summary>
        /// Deserializes the given JSON to an instance of ContentBlocksModelValue
        /// </summary>
        /// <param name="json">JSON to deserialize</param>
        /// <returns></returns>
        public ContentBlocksModelValue Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<ContentBlocksModelValue>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
