using System.Text.RegularExpressions;

namespace com.mosso.cloudfiles.utils
{
    public class ContainerNameValidator
    {
        public const int MAX_CONTAINER_NAME_LENGTH = 256;

        public static bool Validate(string containerName)
        {
            return containerName.IndexOf("?") < 0 && 
                   containerName.IndexOf("/") < 0 &&
                   containerName.Length <= MAX_CONTAINER_NAME_LENGTH;
        }
    }
}