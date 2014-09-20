using System;

namespace Frankstein.Common
{
    /// <summary>
    /// Exception utilizada por CustomErrorHttpModule
    /// Quando esta exception é lançada, a página de conterá os detalhes somente desta exception,
    /// Deve-se exibir uma mensagem significativa ao usuário final sobre a exception.
    /// </summary>
    public class CustomException : Exception
    {
        public CustomException(string msg)
            : base(msg)
        {

        }

        public CustomException(string msg, Exception innerException)
            : base(msg, innerException)
        {

        }
    }
}
