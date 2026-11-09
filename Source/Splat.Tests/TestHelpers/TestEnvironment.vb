#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " TestEnvironment "

Namespace TestHelpers

    ''' <summary>
    ''' Provides checks about the environment where the tests are running.
    ''' </summary>
    Friend Module TestEnvironment

        ''' <summary>
        ''' Marks the current test as inconclusive when it is not running on Windows.
        ''' <para></para>
        ''' Some GDI+ features (e.g. <see cref="System.Drawing.Imaging.ImageAttributes.SetWrapMode(System.Drawing.Drawing2D.WrapMode)"/>)
        ''' are not honored by the libgdiplus implementation used on other platforms.
        ''' </summary>
        Friend Sub RequireWindowsGdiPlus()

            If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                Assert.Inconclusive("This test requires the native Windows GDI+ implementation.")
            End If

        End Sub

    End Module

End Namespace

#End Region
