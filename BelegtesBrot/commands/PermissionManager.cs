using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

using System;
using System.Collections.Generic;

    // Simple permission identifier – can be any ulong value
    using PermissionId = Guid;
    /// <summary>
    /// Manages permissions for subjects and allows access control.
    /// </summary>
    public class PermissionManager
    {
        /// <summary>
        /// A dictionary of permission identifiers mapped to sets of subject identifiers,
        /// used for lookup in the PermissionManager class.
        /// </summary>
        /// <remarks>Stores permissions for subjects, keyed by permission identifier.</remarks>
        /// <remarks>The dictionary is populated through the Add method and updated through the Remove method.</remarks>
        private readonly Dictionary<PermissionId, HashSet<ulong>> _permissions = new();

        /// <summary>
        /// A dictionary of permission identifiers mapped to subject identifiers,
        /// used for reverse lookup in the PermissionManager class.
        /// </summary>
        private readonly Dictionary<ulong,HashSet<PermissionId>> _permissionsReverse = new();

        /// <summary>
        /// Adds a permission for a subject.
        /// </summary>
        /// <param name="permissionId">The identifier of the permission.</param>
        /// <param name="Id">The identifier of the subject.</param>
        public void Add(PermissionId permissionId, ulong Id)
        {
            // ---- permission → subjects ----
            if (!_permissions.TryGetValue(permissionId, out var ids))
            {
                _permissions[permissionId] = [];
            }
            ids.Add(Id);

            // ---- subject → permissions ----
            if (!_permissionsReverse.TryGetValue(Id, out var perms))
            {
                _permissionsReverse[Id] = [permissionId];
            }
            perms.Add(permissionId);
        }

        /// <summary>
        /// Removes a permission from a subject.
        /// </summary>
        /// <param name="Id">The identifier of the subject.</param>
        /// <param name="permissionId">The identifier of the permission.</param>
        /// <returns>True if the permission was removed, false otherwise.</returns>
        public bool Remove(ulong Id, PermissionId permissionId)
        {
            bool removedFromSubject = false;
            bool removedFromPerm = false;

            // Remove from subject → permissions map
            if (_permissionsReverse.TryGetValue(Id, out var perms))
            {
                removedFromSubject = perms.Remove(permissionId);
                if (perms.Count == 0)
                    _permissionsReverse.Remove(Id);
            }

            // Remove from permission → subjects map
            if (_permissions.TryGetValue(permissionId, out var subjects))
            {
                removedFromPerm = subjects.Remove(Id);
                if (subjects.Count == 0)
                    _permissions.Remove(permissionId);
            }

            // Return true only if the permission was actually removed
            return removedFromSubject && removedFromPerm;
        }

        /// <summary>
        /// Determines whether the given subject has a specified permission.
        /// </summary>
        /// <param name="subjectId">The identifier of the subject.</param>
        /// <param name="permission">The identifier of the permission.</param>
        /// <returns>True if the subject has the permission, false otherwise.</returns>
        public bool HasPermission(ulong subjectId, PermissionId permission)
        {
            return _permissions.TryGetValue(permission, out var subjects) &&
                   subjects.Contains(subjectId);
        }

        /// <summary>
        /// Retrieves the list of permissions associated with a given subject.
        /// </summary>
        /// <param name="subjectId">The identifier of the subject.</param>
        /// <return>A collection of permission identifiers if found, otherwise null.</return>
        public IEnumerable<PermissionId>? GetPermissions(ulong subjectId)
        {
            return _permissionsReverse.GetValueOrDefault(subjectId);
        }

        /// <summary>
        /// Retrieves all subjects associated with a given permission.
        /// </summary>
        /// <param name="permission">The identifier of the permission.</param>
        /// <returns>A sequence of subject identifiers that have the specified permission.</returns>
        public IEnumerable<ulong>? GetSubjects(PermissionId permission)
        {
            return _permissions.GetValueOrDefault(permission);
        }
    }
