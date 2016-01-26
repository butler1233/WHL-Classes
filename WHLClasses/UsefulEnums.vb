Imports System.Drawing

Namespace Constants
    Public Module GridHighlightColors
        Public Red As Color = Color.FromArgb(255, 142, 142)
        Public Orange As Color = Color.FromArgb(255, 201, 142)
        Public Green As Color = Color.FromArgb(147, 255, 142)
        Public Blue As Color = Color.FromArgb(142, 142, 255)
    End Module
    Public Module GridDisabledColors
        Public Foreground As Color = Color.FromArgb(165, 165, 165)
        Public Background As Color = Color.FromArgb(230, 230, 230)
    End Module
    Public Module GridEnabledColors
        Public Background As Color = Color.FromArgb(240, 240, 240)
        Public Foreground As Color = Color.FromArgb(15, 15, 15)
    End Module
    Public Module ExplorerColors
        Public ExplorerBackground As Color = Color.FromArgb(255, 255, 255)
        Public ExplorerTextStandard As Color = Color.FromArgb(0, 0, 0)
        Public ExplorerTextRed As Color = Color.FromArgb(250, 0, 0)
    End Module

End Namespace