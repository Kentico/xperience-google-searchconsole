namespace Kentico.Xperience.Google.SearchConsole
{
    /// <summary>
    /// A class which contains constants from Google API responses which are not implemented in the packages.
    /// Includes methods to display the constant's value as a human-friendly icon or text.
    /// </summary>
    public abstract class GoogleApiConstant
    {
        /// <summary>
        /// The constant value provided by the constructor.
        /// </summary>
        public string constantValue;


        /// <summary>
        /// Sets the <see cref="constantValue"/> to be converted into an icon or text.
        /// </summary>
        /// <param name="constantValue">A Google API constant value.</param>
        public GoogleApiConstant(string constantValue)
        {
            this.constantValue = constantValue;
        }


        /// <summary>
        /// Gets a font icon for displaying the constant value to a user.
        /// </summary>
        public abstract string GetIcon();


        /// <summary>
        /// Gets a human-friendly description of the constant value for displaying to a user.
        /// </summary>
        public abstract string GetMessage();
    }
}