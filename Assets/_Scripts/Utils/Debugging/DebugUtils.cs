namespace _Scripts.Utils.Debugging
{
    using MethodBase = System.Reflection.MethodBase;
    using StringBuilder = System.Text.StringBuilder;

    public static class DebugUtils
    {
        public static string GetCleanDeclaringTypeName(MethodBase method)
        {
            if (method == null)
                return null;

            string name;

            if (method?.DeclaringType?.ReflectedType != null)
                name = method.DeclaringType.ReflectedType.Name;
            else
                name = method.DeclaringType.Name;

            StringBuilder result = new StringBuilder(256);

            for (int i = 0; i < name.Length; i++)
            {
                char ch = name[i];

                if (ch == '+')
                {
                    if (i + 1 < name.Length && name[i + 1] != '<')
                        result.Append('.');

                    continue;
                }

                if (ch == '<')
                    break;

                result.Append(ch);
            }

            return result.ToString();
        }

        public static string GetCleanMethodName(MethodBase method)
        {
            if (method == null)
                return null;

            string name;

            if (method.Name.Contains("b__"))
            {
                name = method.Name;
            }
            else if (method.DeclaringType.ReflectedType != null)
            {
                name = method.ReflectedType.Name;

                int startIndex = -1;
                int length = -1;

                for (int i = 0; startIndex < 0 && i < name.Length; i++)
                    if (name[i].Equals('<'))
                        startIndex = i + 1;

                for (int i = name.Length - 1; startIndex >= 0 && length < 0 && i >= 0; i--)
                    if (name[i].Equals('>'))
                        length = i - startIndex;

                if (startIndex > 0)
                    name = name.Substring(startIndex, length);
            }
            else
                name = method.Name;

            return name + "()";
        }
    }
}
