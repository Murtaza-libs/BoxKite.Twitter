﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Service;

namespace BoxKite.Twitter.Modules
{
    public static class ListExtensions
    {
        /// <summary>
        /// Returns all lists the authenticating or specified user subscribes to, including their own. The user is specified using the user_id or screen_name parameters. If no user is given, the authenticating user is used.
        /// </summary>
        /// <param name="user_id">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="screen_name">The screen name of the user for whom to return results for.</param>
        /// <param name="reverse">Set this to true if you would like owned lists to be returned first.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/list </remarks>
        public static async Task<IEnumerable<TwitterList>> GetLists(this IUserSession session, int user_id=0, string screen_name ="", bool reverse = false)
        {
            var parameters = new SortedDictionary<string, string> {{"reverse", reverse.ToString()}};

            if (user_id > 0)
                parameters.Add("user_id",user_id.ToString());

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            var url = Api.Resolve("/1.1/lists/list.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToTwitterLists());
        }

        /// <summary>
        /// Returns a timeline of tweets authored by members of the specified list. Retweets are included by default.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="since_id">Returns results with an ID greater than (that is, more recent than) the specified ID.</param>
        /// <param name="count">Specifies the number of results to retrieve per "page."</param>
        /// <param name="max_id">Returns results with an ID less than (that is, older than) or equal to the specified ID.</param>
        /// <param name="include_rts">the list timeline will contain native retweets (if they exist) in addition to the standard stream of tweets.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/statuses </remarks>
        public static async Task<IEnumerable<Tweet>> GetListTimeline(this IUserSession session, int list_id, string slug, int owner_id = 0, string owner_screen_name = "", long since_id = 0, int count = 200, long max_id = 0, bool include_rts = true)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                     {"include_rts", include_rts.ToString()},
                                     {"count", count.ToString()},
                                 };

            if (since_id > 0)
            {
                parameters.Add("since_id", since_id.ToString());
            }

            if (max_id > 0)
            {
                parameters.Add("max_id", max_id.ToString());
            }

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            var url = Api.Resolve("/1.1/lists/statuses.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToListOfTweets());
        }

        /// <summary>
        /// Removes the specified member from the list. The authenticated user must be the list's owner to remove members from the list.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="user_id">The ID of the user to remove from the list. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="screen_name">The screen name of the user for whom to remove from the list. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/members/destroy </remarks>
        public static async Task<bool> DeleteUserFromList(this IUserSession session, int list_id, string slug,
            int user_id = 0, string screen_name = "", string owner_screen_name = "", int owner_id = 0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            var url = Api.Resolve("/1.1/lists/members/destroy");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToBoolean());
        }

        /// <summary>
        /// Returns the lists the specified user has been added to. If user_id or screen_name are not provided the memberships for the authenticating user are returned.
        /// </summary>
        /// <param name="user_id">The ID of the user for whom to return results for.</param>
        /// <param name="screen_name">The screen name of the user for whom to return results for.</param>
        /// <param name="cursor">Breaks the results into pages. Provide a value of -1 to begin paging.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/memberships </remarks>
        public static async Task<UserInListCursored> GetListMembershipForUser(this IUserSession session, int user_id = 0,
            string screen_name = "", int cursor = -1)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"cursor", cursor.ToString()},
                                 };

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            var url = Api.Resolve("/1.1/lists/memberships.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToUserInListCursored()); 
        }

        /// <summary>
        /// Returns the subscribers of the specified list. Private list subscribers will only be shown if the authenticated user owns the specified list.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="cursor">Breaks the results into pages.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/subscribers </remarks>
        public static async Task<UserListDetailedCursored> GetListSubscribers(this IUserSession session, int list_id,
            string slug, int owner_id = 0,
            string owner_screen_name = "", int cursor = -1)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"cursor", cursor.ToString()},
                                     {"list_id", list_id.ToString()},
                                     {"include_entities", false.ToString()},
                                     {"skip_status", false.ToString()},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            if (!string.IsNullOrWhiteSpace(slug))
            {
                parameters.Add("slug", slug);
            }

            var url = Api.Resolve("/1.1/lists/subscribers.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToUserListDetailedCursored()); 
        }

        /// <summary>
        /// Subscribes the authenticated user to the specified list.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/subscribers/create </remarks>
        public static async Task<TwitterList> SubscribeToUsersList(this IUserSession session, int list_id,
            string slug, int owner_id = 0, string owner_screen_name = "")
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            if (!string.IsNullOrWhiteSpace(slug))
            {
                parameters.Add("slug", slug);
            }
            var url = Api.Resolve("/1.1/lists/subscribers/create.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToTwitterList());
        }

        /// <summary>
        /// Check if the specified user is a subscriber of the specified list. Returns the user if they are subscriber.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="user_id">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="screen_name">The screen name of the user for whom to return results for. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/subscribers/show </remarks>
        public static async Task<User> IsSubscribedToList(this IUserSession session, int list_id, string slug,
    int user_id = 0, string screen_name = "", string owner_screen_name = "", int owner_id = 0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            var url = Api.Resolve("/1.1/lists/subscribers/show.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingleUser());
        }

        /// <summary>
        /// Unsubscribes the authenticated user from the specified list.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/subscribers/destroy </remarks>
        public static async Task<bool> DeleteFromUsersList(this IUserSession session, int list_id, string slug,
            string owner_screen_name = "", int owner_id = 0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            var url = Api.Resolve("/1.1/lists/subscribers/destroy.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToBoolean());
        }
        
        /// <summary>
        /// Adds multiple members to a list (up to 100)
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="screen_names">list of screen names, up to 100 are allowed in a single request.</param>
        /// <param name="user_ids">list of user IDs, up to 100 are allowed in a single request.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/members/create_all </remarks>
        public static async Task<bool> AddUsersToList(this IUserSession session, int list_id, string slug,
            IEnumerable<string> screen_names, IEnumerable<int> user_ids,
            string owner_screen_name = "", int owner_id = 0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            var userIDList = new StringBuilder();
            if (user_ids.HasAny())
            {
                foreach (var userID in user_ids)
                {
                    userIDList.Append(userID + ",");
                }
                parameters.Add("user_id", userIDList.ToString().TrimLastChar());
            }

            var screenNameList = new StringBuilder();
            if (screen_names.HasAny())
            {
                foreach (var screenname in screen_names)
                {
                    screenNameList.Append(screenname + ",");
                }
                parameters.Add("screen_name", screenNameList.ToString().TrimLastChar());
            }

            var url = Api.Resolve("/1.1/lists/members/create_all.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToBoolean());
        }

        /// <summary>
        /// Check if the specified user is a member of the specified list
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="user_id">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="screen_name">The screen name of the user for whom to return results for. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/members/show </remarks>
        public static async Task<User> IsUserOnList(this IUserSession session, int list_id, string slug,
            int user_id = 0, string screen_name = "", string owner_screen_name = "", int owner_id = 0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                     {"include_entities", false.ToString()},
                                     {"skip_status",true.ToString()}
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            var url = Api.Resolve("/1.1/lists/members/show.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToSingleUser());
        }

        /// <summary>
        /// Returns the members of the specified list. Private list members will only be shown if the authenticated user owns the specified list.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
       /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="cursor">Breaks the results into pages.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/members </remarks>
        public static async Task<UserListDetailedCursored> GetMembersOnList(this IUserSession session, int list_id, string slug,
            string owner_screen_name = "", int owner_id = 0, int cursor = -1)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                     {"include_entities", false.ToString()},
                                     {"skip_status",true.ToString()}
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            var url = Api.Resolve("/1.1/lists/members.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToUserListDetailedCursored());
        }

        /// <summary>
        /// Add a member to a list. The authenticated user must own the list to be able to add members to it. 
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="screen_name_to_add">The screen name of the user for whom to return results for. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="user_id_to_add">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/members/create </remarks>
        public static async Task<bool> AddUserToMyList(this IUserSession session, int list_id, string slug,
    string screen_name_to_add, int user_id_to_add, string owner_screen_name = "", int owner_id = 0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            if (user_id_to_add > 0)
            {
                parameters.Add("user_id", user_id_to_add.ToString());
            }

            if (!string.IsNullOrWhiteSpace(screen_name_to_add))
            {
                parameters.Add("screen_name", screen_name_to_add);
            }

            var url = Api.Resolve("/1.1/lists/members/create.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToBoolean());
        }

        /// <summary>
        /// Deletes the specified list. The authenticated user must own the list to be able to destroy it.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
         /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/destroy </remarks>
        public static async Task<TwitterList> DeleteList(this IUserSession session, int list_id,
             string slug, int owner_id = 0, string owner_screen_name = "")
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            var url = Api.Resolve("/1.1/lists/destroy.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToTwitterList());
        }

        /// <summary>
        /// Updates the specified list. The authenticated user must own the list to be able to update it.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="name">The name for the list.</param>
        /// <param name="mode">Whether your list is public or private. Values can be public or private. If no mode is specified the list will be public.</param>
        /// <param name="description">The description to give the list.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/update </remarks>
        public static async Task<Boolean> ChangeList(this IUserSession session, int list_id,
            string slug, string name = "", string mode = "", string description = "", int owner_id = 0,
            string owner_screen_name = "")
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                parameters.Add("name", name);
            }

            if (!string.IsNullOrWhiteSpace(mode))
            {
                parameters.Add("mode", mode);
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                parameters.Add("description", description);
            }

            var url = Api.Resolve("/1.1/lists/update.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToBoolean());
        }

        /// <summary>
        /// Creates a new list for the authenticated user. Note that you can't create more than 20 lists per account.
        /// </summary>
        /// <param name="name">The name for the list.</param>
        /// <param name="mode">Whether your list is public or private. Values can be public or private. If no mode is specified the list will be public.</param>
        /// <param name="description">The description to give the list.</param>
         /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/update </remarks>
        public static async Task<TwitterList> CreateList(this IUserSession session, string name, string mode, string description = "", int owner_id = 0,
            string owner_screen_name = "")
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"name", name},
                                     {"mode", mode},
                                 };

            if (!string.IsNullOrWhiteSpace(description))
            {
                parameters.Add("description", description);
            }

            var url = Api.Resolve("/1.1/lists/create.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToTwitterList());
        }

        /// <summary>
        /// Returns the specified list. Private lists will only be shown if the authenticated user owns the specified list.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/show </remarks>
        public static async Task<TwitterList> GetList(this IUserSession session, int list_id, string slug,
            string owner_screen_name = "", int owner_id = 0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }
            
            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            var url = Api.Resolve("/1.1/lists/show.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToTwitterList());
        }

        /// <summary>
        /// Obtain a collection of the lists the specified user is subscribed to, 20 lists per page by default. Does not include the user's own lists.
        /// </summary>
        /// <param name="screen_name">The screen name of the user for whom to return results for. Helpful for disambiguating when a valid screen name is also a user ID.</param>
        /// <param name="user_id">The ID of the user for whom to return results for. Helpful for disambiguating when a valid user ID is also a valid screen name.</param>
        /// <param name="count">The amount of results to return per page. Defaults to 20. No more than 1000 results will ever be returned in a single page.</param>
        /// <param name="cursor">Breaks the results into pages. Provide a value of -1 to begin paging.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/subscriptions </remarks>
        public static async Task<TwitterListCursored> GetMySubscriptions(this IUserSession session, 
            string screen_name = "", int user_id = 0, int count=20, int cursor= -1)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", count.ToString()},
                                     {"cursor", cursor.ToString()},
                                 };

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            var url = Api.Resolve("/1.1/lists/subscriptions.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToTwitterListCursored());
        }

        /// <summary>
        /// Removes multiple members from a list, by specifying a comma-separated list of member ids or screen names. The authenticated user must own the list to be able to remove members from it.
        /// </summary>
        /// <param name="list_id">The numerical id of the list.</param>
        /// <param name="slug">You can identify a list by its slug instead of its numerical id. If you decide to do so, note that you'll also have to specify the list owner using the owner_id or owner_screen_name parameters.</param>
        /// <param name="screen_names">list of screen names, up to 100 are allowed in a single request.</param>
        /// <param name="user_ids">list of user IDs, up to 100 are allowed in a single request.</param>
        /// <param name="owner_screen_name">The screen name of the user who owns the list being requested by a slug.</param>
        /// <param name="owner_id">The user ID of the user who owns the list being requested by a slug.</param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/post/lists/members/destroy_all </remarks>
        public static async Task<bool> DeleteUsersFromList(this IUserSession session, int list_id, string slug,
            IEnumerable<string> screen_names, IEnumerable<int> user_ids,
            string owner_screen_name = "", int owner_id = 0)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"list_id", list_id.ToString()},
                                     {"slug", slug},
                                 };

            if (owner_id > 0)
            {
                parameters.Add("owner_id", owner_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(owner_screen_name))
            {
                parameters.Add("owner_screen_name", owner_screen_name);
            }

            var userIDList = new StringBuilder();
            if (user_ids.HasAny())
            {
                foreach (var userID in user_ids)
                {
                    userIDList.Append(userID + ",");
                }
                parameters.Add("user_id", userIDList.ToString().TrimLastChar());
            }

            var screenNameList = new StringBuilder();
            if (screen_names.HasAny())
            {
                foreach (var screenname in screen_names)
                {
                    screenNameList.Append(screenname + ",");
                }
                parameters.Add("screen_name", screenNameList.ToString().TrimLastChar());
            }

            var url = Api.Resolve("/1.1/lists/members/destroy_all.json");
            return await session.PostAsync(url, parameters)
                          .ContinueWith(c => c.MapToBoolean());
        }

        /// <summary>
        /// Returns the lists owned by the specified Twitter user. Private lists will only be shown if the authenticated user is also the owner of the lists.
        /// </summary>
        /// <param name="screen_name"></param>
        /// <param name="user_id"></param>
        /// <param name="count"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        /// <remarks> ref: https://dev.twitter.com/docs/api/1.1/get/lists/ownerships </remarks>
        public static async Task<TwitterListCursored> GetListOwned(this IUserSession session,
            string screen_name = "", int user_id = 0, int count = 20, int cursor = -1)
        {
            var parameters = new SortedDictionary<string, string>
                                 {
                                     {"count", count.ToString()},
                                     {"cursor", cursor.ToString()},
                                 };

            if (user_id > 0)
            {
                parameters.Add("user_id", user_id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(screen_name))
            {
                parameters.Add("screen_name", screen_name);
            }

            var url = Api.Resolve("/1.1/lists/ownerships.json");
            return await session.GetAsync(url, parameters)
                          .ContinueWith(c => c.MapToTwitterListCursored());
        }

    }
}
