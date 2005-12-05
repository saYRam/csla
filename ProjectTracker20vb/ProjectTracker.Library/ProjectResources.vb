Imports System.Data.SqlClient

<Serializable()> _
Public Class ProjectResources
  Inherits BusinessListBase(Of ProjectResources, ProjectResource)

#Region " Business Methods "

  Default Public Overloads ReadOnly Property Item(ByVal resourceId As Integer) As ProjectResource
    Get
      For Each res As ProjectResource In Me
        If res.ResourceId = resourceId Then
          Return res
        End If
      Next
      Return Nothing
    End Get
  End Property

  Public Sub Assign(ByVal resourceId As Integer)

    DoAssignment(ProjectResource.NewProjectResource(resourceId, RoleList.DefaultRole))

  End Sub

  Private Sub DoAssignment(ByVal resource As ProjectResource)

    If Not Contains(resource) Then
      Me.Add(resource)

    Else
      Throw New InvalidOperationException("Resource already assigned to project")
    End If

  End Sub

  Public Overloads Sub Remove(ByVal resourceId As Integer)

    For Each res As ProjectResource In Me
      If res.ResourceId = resourceId Then
        Remove(res)
        Exit For
      End If
    Next

  End Sub

#End Region

#Region " Contains "

  Public Overloads Function Contains( _
      ByVal resourceId As Integer) As Boolean

    For Each res As ProjectResource In Me
      If res.ResourceId = resourceId Then
        Return True
      End If
    Next

    Return False

  End Function

  Public Overloads Function ContainsDeleted( _
    ByVal resourceId As Integer) As Boolean

    For Each res As ProjectResource In DeletedList
      If res.ResourceId = resourceId Then
        Return True
      End If
    Next

    Return False

  End Function

#End Region

#Region " Factory Methods "

  Friend Shared Function NewProjectResources() As ProjectResources

    Return New ProjectResources

  End Function

  Friend Shared Function GetProjectResources( _
    ByVal dr As SafeDataReader) As ProjectResources

    Return New ProjectResources(dr)

  End Function

#End Region

#Region " Constructors "

  Private Sub New()

    MarkAsChild()

  End Sub

#End Region

#Region " Data Access "

  ' called to load data from the database
  Private Sub New(ByVal dr As SafeDataReader)

    MarkAsChild()
    While dr.Read()
      Me.Add(ProjectResource.GetResource(dr))
    End While

  End Sub

  Friend Sub Update(ByVal project As Project)

    Dim obj As ProjectResource

    ' update (thus deleting) any deleted child objects
    For Each obj In DeletedList
      obj.DeleteSelf(project)
    Next
    ' now that they are deleted, remove them from memory too
    DeletedList.Clear()

    ' add/update any current child objects
    For Each obj In Me
      If obj.IsNew Then
        obj.Insert(project)

      Else
        obj.Update(project)
      End If
    Next

  End Sub

#End Region

End Class
