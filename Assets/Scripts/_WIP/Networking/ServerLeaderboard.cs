// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// namespace _WIP.Networking
// {
//     public class ServerLeaderboard : NetworkBehaviour
//     {
//         // TODO: Make private to improve reliability.
//         public readonly SyncDictionary<uint, float> netIdPoints = new SyncDictionary<uint, float>();
//     
//         public event Action<uint> PlayerAddSynced;
//         public event Action<uint> PlayerRemoveSynced;
//         public event PointsChangeHandler PointsChangeSynced;
//         public event Action ClearSynced;
//     
//         private void Awake()
//         {
//             netIdPoints.Callback += (operation, key, item) =>
//             {
//                 switch (operation)
//                 {
//                     case SyncIDictionary<uint, float>.Operation.OP_ADD:
//                         PlayerAddSynced.Invoke(key);
//                         break;
//                     case SyncIDictionary<uint, float>.Operation.OP_REMOVE:
//                         PlayerRemoveSynced.Invoke(key);
//                         break;
//                     case SyncIDictionary<uint, float>.Operation.OP_CLEAR:
//                         ClearSynced.Invoke();
//                         break;
//                     case SyncIDictionary<uint, float>.Operation.OP_SET:
//                         PointsChangeSynced.Invoke(key, item);
//                         break;
//                     default:
//                         throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
//                 }
//             };
//         }
//     
//         // public bool TryGetPlayerPlace(uint playerNetId, TopPlayersBar.SortingToTop sortType, out int playerPlace)
//         // {
//         //     if (HasPlayer(playerNetId))
//         //     {
//         //         playerPlace = GetPlayerPlace(playerNetId, sortType);
//         //
//         //         return true;
//         //     }
//         //     else
//         //     {
//         //         playerPlace = -1;
//         //
//         //         return false;
//         //     }
//         // }
//     
//         public int GetPlayerPlace(uint playerNetId, TopPlayersBar.SortingToTop sortType)
//         {
//             List<KeyValuePair<uint, float>> sortedNetIdPoints = sortType == TopPlayersBar.SortingToTop.maximum
//                 ? netIdPoints.OrderByDescending(s => s.Value).ToList()
//                 : netIdPoints.OrderBy(s => s.Value).ToList();
//             KeyValuePair<uint, float> playerScore = sortedNetIdPoints.FirstOrDefault(x => x.Key == playerNetId);
//     
//             return sortedNetIdPoints.IndexOf(playerScore) + 1;
//         }
//     
//         public bool HasPlayer(uint playerNetId)
//         {
//             return netIdPoints.ContainsKey(playerNetId);
//         }
//     
//         [Server]
//         public void AddPlayer(uint playerNetId, float defaultPoints = 0)
//         {
//             netIdPoints.Add(playerNetId, defaultPoints);
//         }
//     
//         [Server]
//         public void RemovePlayerIfExists(uint playerNetId)
//         {
//             if (HasPlayer(playerNetId))
//                 RemovePlayer(playerNetId);
//         }
//     
//         [Server]
//         public void RemovePlayer(uint playerNetId)
//         {
//             netIdPoints.Remove(playerNetId);
//         }
//     
//         /// <summary>
//         /// Also works if dictionary doesn't contains key.
//         /// In this case, Callback will be fired with Operation.OP_ADD parameter.
//         /// </summary>
//         [Command(requiresAuthority = false)]
//         public void CmdChangePoints(float points, ModePlayer sender)
//         {
//             ChangePoints(sender.netId, points);
//         }
//     
//         /// <summary>
//         /// Also works if dictionary doesn't contains key.
//         /// In this case, Callback will be fired with Operation.OP_ADD parameter.
//         /// </summary>
//         [Server]
//         public void ChangePoints(uint playerNetId, float points)
//         {
//             netIdPoints[playerNetId] = points;
//         }
//     
//         [Command(requiresAuthority = false)]
//         public void CmdAddPoints(float points, ModePlayer sender)
//         {
//             if (HasPlayer(sender.netId))
//                 AddPoints(sender.netId, points);
//             else
//                 AddPlayer(sender.netId, points);
//         }
//     
//         [Server]
//         public void AddPoints(uint playerNetId, float points)
//         {
//             netIdPoints[playerNetId] += points;
//         }
//     
//         [Server]
//         public void ResetAllPlayerPoints(float defaultValue = 0)
//         {
//             foreach (uint playerNetId in netIdPoints.Keys)
//                 netIdPoints[playerNetId] = defaultValue;
//         }
//     
//         [Server]
//         public void Clear()
//         {
//             netIdPoints.Clear();
//         }
//     }
//     
//     public delegate void PointsChangeHandler(uint playerNetId, float newPoints);
// }