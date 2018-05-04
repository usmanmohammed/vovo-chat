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
    public class ChatsController : ApiController
    {
        private VovoAPIContext db = new VovoAPIContext();

        // GET: api/Chats
        public IQueryable<Chat> GetChats()
        {
            return db.Chats;
        }

        // GET: api/Chats/5
        [ResponseType(typeof(Chat))]
        public async Task<IHttpActionResult> GetChat(int id)
        {
            Chat chat = await db.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            return Ok(chat);
        }

        // PUT: api/Chats/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutChat(int id, Chat chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chat.ChatID)
            {
                return BadRequest();
            }

            db.Entry(chat).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatExists(id))
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

        // POST: api/Chats
        [ResponseType(typeof(Chat))]
        public async Task<IHttpActionResult> PostChat(Chat chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Chats.Add(chat);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = chat.ChatID }, chat);
        }

        // DELETE: api/Chats/5
        [ResponseType(typeof(Chat))]
        public async Task<IHttpActionResult> DeleteChat(int id)
        {
            Chat chat = await db.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            db.Chats.Remove(chat);
            await db.SaveChangesAsync();

            return Ok(chat);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ChatExists(int id)
        {
            return db.Chats.Count(e => e.ChatID == id) > 0;
        }
    }
}