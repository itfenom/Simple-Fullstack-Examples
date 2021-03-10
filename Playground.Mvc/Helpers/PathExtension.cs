namespace Playground.Mvc.Helpers
{
    public static class PathExtension
    {
        public static string WithTrailingSlash(this string self)
        {
            if (!string.IsNullOrEmpty(self) && self.EndsWith("/"))
            {
                return self;
            }

            return (self ?? string.Empty) + "/";
        }
    }
}