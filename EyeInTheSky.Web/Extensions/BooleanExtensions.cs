namespace EyeInTheSky.Web.Extensions
{
    public static class BooleanExtensions
    {

        public static string ToIcon(this bool data)
        {
            return data
                ? "<i class=\"text-success fas fa-check-circle\"></i>"
                : "<i class=\"text-danger fas fa-times-circle\"></i>";
        }
    }
}