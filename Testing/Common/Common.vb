Imports Microsoft.VisualStudio.TestTools.UnitTesting
Public Module Common
    Public Sub ExpectException(Of E As Exception)(action As Action)
        Try
            Call action()
        Catch ex As E
            Return
        Catch ex As Exception
            Assert.Fail("Expected an exception of type " + GetType(E).ToString + " but hit an exception type " + ex.GetType.ToString)
        End Try
        Assert.Fail("Expected an exception of type " + GetType(E).ToString + " but hit no exception.")
    End Sub
End Module