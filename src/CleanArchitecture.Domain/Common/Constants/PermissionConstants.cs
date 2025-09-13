namespace CleanArchitecture.Domain.Common.Constants
{
  /// <summary>
  /// Constantes para todos los permisos del sistema
  /// Centraliza los nombres de permisos para evitar errores de tipeo y facilitar el mantenimiento
  /// </summary>
  public static class PermissionConstants
  {
    #region Users Permissions

    /// <summary>
    /// Permisos relacionados con la gestión de usuarios
    /// </summary>
    public static class Users
    {
      /// <summary>
      /// Permiso para leer/listar usuarios
      /// </summary>
      public const string Read = "Users.Read";

      /// <summary>
      /// Permiso para crear nuevos usuarios
      /// </summary>
      public const string Write = "Users.Write";

      /// <summary>
      /// Permiso para actualizar usuarios existentes
      /// </summary>
      public const string Update = "Users.Update";

      /// <summary>
      /// Permiso para eliminar usuarios
      /// </summary>
      public const string Delete = "Users.Delete";

      /// <summary>
      /// Permiso para gestionar roles de usuarios
      /// </summary>
      public const string ManageRoles = "Users.ManageRoles";

      /// <summary>
      /// Permiso para ver información sensible de usuarios
      /// </summary>
      public const string ViewSensitive = "Users.ViewSensitive";
    }

    #endregion

    #region Roles Permissions

    /// <summary>
    /// Permisos relacionados con la gestión de roles
    /// </summary>
    public static class Roles
    {
      /// <summary>
      /// Permiso para leer/listar roles
      /// </summary>
      public const string Read = "Roles.Read";

      /// <summary>
      /// Permiso para crear nuevos roles
      /// </summary>
      public const string Write = "Roles.Write";

      /// <summary>
      /// Permiso para actualizar roles existentes
      /// </summary>
      public const string Update = "Roles.Update";

      /// <summary>
      /// Permiso para eliminar roles
      /// </summary>
      public const string Delete = "Roles.Delete";

      /// <summary>
      /// Permiso para gestionar permisos de roles
      /// </summary>
      public const string ManagePermissions = "Roles.ManagePermissions";
    }

    #endregion

    #region Permissions Permissions

    /// <summary>
    /// Permisos relacionados con la gestión de permisos
    /// </summary>
    public static class Permissions
    {
      /// <summary>
      /// Permiso para leer/listar permisos
      /// </summary>
      public const string Read = "Permissions.Read";

      /// <summary>
      /// Permiso para crear nuevos permisos
      /// </summary>
      public const string Write = "Permissions.Write";

      /// <summary>
      /// Permiso para actualizar permisos existentes
      /// </summary>
      public const string Update = "Permissions.Update";

      /// <summary>
      /// Permiso para eliminar permisos
      /// </summary>
      public const string Delete = "Permissions.Delete";
    }

    #endregion

    #region System Permissions

    /// <summary>
    /// Permisos relacionados con la administración del sistema
    /// </summary>
    public static class System
    {
      /// <summary>
      /// Permiso para acceder al panel de administración
      /// </summary>
      public const string Admin = "System.Admin";

      /// <summary>
      /// Permiso para ver logs del sistema
      /// </summary>
      public const string ViewLogs = "System.ViewLogs";

      /// <summary>
      /// Permiso para gestionar configuraciones del sistema
      /// </summary>
      public const string ManageSettings = "System.ManageSettings";

      /// <summary>
      /// Permiso para ejecutar tareas de mantenimiento
      /// </summary>
      public const string Maintenance = "System.Maintenance";
    }

    #endregion

    #region Audit Permissions

    /// <summary>
    /// Permisos relacionados con auditoría y reportes
    /// </summary>
    public static class Audit
    {
      /// <summary>
      /// Permiso para ver reportes de auditoría
      /// </summary>
      public const string ViewReports = "Audit.ViewReports";

      /// <summary>
      /// Permiso para exportar datos de auditoría
      /// </summary>
      public const string ExportData = "Audit.ExportData";
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Obtiene todos los permisos del sistema como una lista
    /// </summary>
    /// <returns>Lista de todos los permisos disponibles</returns>
    public static List<string> GetAllPermissions()
    {
      return new List<string>
            {
                // Users permissions
                Users.Read,
                Users.Write,
                Users.Update,
                Users.Delete,
                Users.ManageRoles,
                Users.ViewSensitive,
                
                // Roles permissions
                Roles.Read,
                Roles.Write,
                Roles.Update,
                Roles.Delete,
                Roles.ManagePermissions,
                
                // Permissions permissions
                Permissions.Read,
                Permissions.Write,
                Permissions.Update,
                Permissions.Delete,
                
                // System permissions
                System.Admin,
                System.ViewLogs,
                System.ManageSettings,
                System.Maintenance,
                
                // Audit permissions
                Audit.ViewReports,
                Audit.ExportData
            };
    }

    /// <summary>
    /// Obtiene los permisos básicos que debe tener un usuario estándar
    /// </summary>
    /// <returns>Lista de permisos básicos</returns>
    public static List<string> GetBasicUserPermissions()
    {
      return new List<string>
            {
                Users.Read, // Puede ver su propio perfil y listar usuarios básicos
            };
    }

    /// <summary>
    /// Obtiene los permisos que debe tener un administrador
    /// </summary>
    /// <returns>Lista de permisos de administrador</returns>
    public static List<string> GetAdminPermissions()
    {
      return GetAllPermissions();
    }

    /// <summary>
    /// Obtiene los permisos que debe tener un moderador
    /// </summary>
    /// <returns>Lista de permisos de moderador</returns>
    public static List<string> GetModeratorPermissions()
    {
      return new List<string>
            {
                Users.Read,
                Users.Write,
                Users.Update,
                Roles.Read,
                Permissions.Read,
                Audit.ViewReports
            };
    }

    /// <summary>
    /// Verifica si un permiso existe en el sistema
    /// </summary>
    /// <param name="permission">Permiso a verificar</param>
    /// <returns>True si el permiso existe, False en caso contrario</returns>
    public static bool IsValidPermission(string permission)
    {
      return GetAllPermissions().Contains(permission);
    }

    #endregion
  }
}
