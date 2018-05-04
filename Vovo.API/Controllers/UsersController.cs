using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vovo.Models;

namespace Vovo.API.Controllers
{
    public class UsersController : ApiController
    {
        private VovoAPIContext db = new VovoAPIContext();

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        [Route("api/users/SearchUsers")]
        [HttpGet]
        public IQueryable<User> SearchUsers(string id)
        {
            return db.Users.Where(r => r.UserID.Contains(id) || r.FullName.Contains(id));
        }

        [Route("api/users/GetFriends")]
        [HttpGet]
        public IQueryable<UserFriend> GetFriends(string id)
        {
            return db.Friends.Where(r => r.UserUserID == id);
        }

        [Route("api/users/GetChats")]
        [HttpGet]
        public IQueryable<Chat> GetChats(string id)
        {
            return db.Chats.Where(r => r.SenderUserID == id || r.ReceiverUserID == id);
        }

        // POST: api/Users
        [Route("api/users/AddFriend")]
        [HttpPost]
        [ResponseType(typeof(UserFriend))]
        public async Task<IHttpActionResult> AddFriend(UserFriend friend)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            friend.DateCreated = DateTime.UtcNow;
            db.Friends.Add(friend);

            try
            {
                await db.SaveChangesAsync();
            }

            catch (DbUpdateException)
            {
                throw;
            }

            return Ok(friend);
        }

        [Route("api/users/AccountTaken")]
        [HttpGet]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> AccountTaken(string id)
        {
            return Ok(await db.Users.CountAsync(e => e.UserID == id) > 0);
        }

        // GET: api/Users/5
        [Route("api/users/login")]
        [HttpGet]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Login(string un, string pw)
        {
            User user = await db.Users.FindAsync(un);
            if (user == null)
            {
                return NotFound();
            }

            if (user.PasswordHash != pw)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(string id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserID)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.DateCreated = DateTime.UtcNow;
            user.IsActive = true;
            db.Users.Add(user);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.UserID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = user.UserID }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        [Route("api/users/UpdateUserImage")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UpdateUserImage(UserImage userImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!UserImageExists(userImage.UserID))
                {
                    db.UserImages.Add(userImage);
                }

                else
                {
                    userImage.DateModified = DateTime.UtcNow;
                    db.Entry(userImage).State = EntityState.Modified;
                }

                await db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("api/users/CreateGetChat")]
        [ResponseType(typeof(Chat))]
        public async Task<IHttpActionResult> CreateGetChat(Chat chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!ChatExists(chat.SenderUserID, chat.ReceiverUserID))
                {
                    db.Chats.Add(chat);
                    await db.SaveChangesAsync();
                }

                else
                {
                    chat = await db.Chats
                        .FirstOrDefaultAsync(r => (r.SenderUserID == chat.SenderUserID && r.ReceiverUserID == chat.ReceiverUserID) ||
                        (r.SenderUserID == chat.ReceiverUserID && r.ReceiverUserID == chat.SenderUserID));
                }

                return Ok(chat);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("api/users/PostMessage")]
        [ResponseType(typeof(Chat))]
        public async Task<IHttpActionResult> PostMessage(Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var chat = await db.Chats.FindAsync(message.ChatID);
                chat.LastMessage = message.Content;

                if (chat.Messages != null && chat.Messages.Count > 0)
                {
                    chat.Messages.Add(message);
                }

                else
                {
                    chat.Messages = new List<Message> { message };
                }

                db.Entry(chat).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok(message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        private bool ChatExists(string id, string friendId)
        {
            return db.Chats.Count(r => r.SenderUserID == id && r.ReceiverUserID == friendId) > 0;

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(string id)
        {
            return db.Users.Count(e => e.UserID == id) > 0;
        }

        private bool UserImageExists(string id)
        {
            return db.UserImages.Count(e => e.UserID == id) > 0;
        }
    }
}