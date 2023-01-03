using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace VerifyTests;

//https://github.com/VerifyTests/Verify.WinForms/blob/main/src/Verify.WinForms/VerifyWinForms.cs

public static class VerifyWinForms
{
    public static void Enable()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterFileConverter<Form>(FormToImage);
        VerifierSettings.RegisterFileConverter<ContextMenuStrip>(MenuToImage);
        VerifierSettings.RegisterFileConverter<UserControl>(ControlToImage);
        VerifierSettings.RegisterFileConverter<Control>(ControlToImage);
    }

    static ConversionResult MenuToImage(ContextMenuStrip control, IReadOnlyDictionary<string, object> context)
    {
        using var form = new Form
        {
            Width = control.Width,
            Height = control.Height,
            ContextMenuStrip = control,
            ShowInTaskbar = false,
            TopLevel = false,
            AutoScaleMode = AutoScaleMode.None
        };
        control.TopLevel = false;
        control.Show();
        return new(null, "png", ControlToImage(control));
    }

    static ConversionResult FormToImage(Form form, IReadOnlyDictionary<string, object> context) =>
        new(null, "png", FormToStream(form));

    static ConversionResult ControlToImage(Control control, IReadOnlyDictionary<string, object> context)
    {
        using var form = new Form
        {
            Width = control.Width,
            Height = control.Height,
            ShowInTaskbar = false,
            TopLevel = false,
            AutoScaleMode = AutoScaleMode.None
        };
        form.Controls.Add(control);
        form.Show();
        return new(null, "png", ControlToImage(control));
    }

    static Stream FormToStream(Form form)
    {
        form.ShowInTaskbar = false;
        form.TopLevel = false;
        form.AutoScaleMode = AutoScaleMode.None;
        form.Show();
        return ControlToImage(form);
    }

    static Stream ControlToImage(Control control)
    {
        using var bitmap = new Bitmap(control.Width, control.Height, PixelFormat.Format32bppArgb);
        control.DrawToBitmap(bitmap, new(0, 0, control.Width, control.Height));
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream;
    }
}