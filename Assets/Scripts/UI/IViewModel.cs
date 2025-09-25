namespace FocusFounder.UI
{
    /// <summary>
    /// Base interface for all ViewModels in the MVVM pattern
    /// </summary>
    public interface IViewModel
    {
        void Bind();
        void Unbind();
    }
}