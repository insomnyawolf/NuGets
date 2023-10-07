using SaucenaoSearch;

namespace SaucenaoSearchTest
{
    public class UnitTest1
    {
        private static HttpClient HttpClient = new HttpClient();
        private static SaucenaoWebInterface SaucenaoWebInterface = new SaucenaoWebInterface(HttpClient);

        [Fact]
        public async Task Test0()
        {
        }

        [Theory]
        [InlineData("https://pbs.twimg.com/media/EDIfKO_VUAAOlbI?format=jpg&name=large")]
        public async Task Test1(string imageUrl)
        {
            var sauces = await SaucenaoWebInterface.GetSauceAsync(imageUrl);

            var sauce = sauces.GetClosestMatch();

            Assert.NotNull(sauce);
        }
    }
}