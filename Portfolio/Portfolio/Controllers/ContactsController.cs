using System.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Portfolio.Models;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ContactsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetContacts()
    {
        try
        {
            string query = @"select id, name, email, message from contacts";

            List<Contact> contacts = new List<Contact>();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            await using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                myCon.Open();
                await using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    var myReader = myCommand.ExecuteReader();

                    while (myReader.Read())
                    {
                        Contact contact = new Contact();
                        contact.Id = Convert.ToInt32(myReader["id"]);
                        contact.Name = myReader["name"].ToString();
                        contact.Email = myReader["email"].ToString();
                        contact.Message = myReader["message"].ToString();

                        contacts.Add(contact);
                    }

                    myReader.Close();
                    await myCon.CloseAsync();
                }
            }

            return Ok(contacts);
        }
        catch (Exception ex)
        {
            //log error
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddContacts(Contact contact)
    {
        try
        {
            string query = @"insert into contacts (name, email, message) values (@Name, @Email, @Message)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("Default");
            await using (MySqlConnection myCon = new MySqlConnection(sqlDataSource))
            {
                myCon.Open();
                await using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Name", contact.Name);
                    myCommand.Parameters.AddWithValue("@Email", contact.Email);
                    myCommand.Parameters.AddWithValue("@Message", contact.Message);
                    var myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    await myCon.CloseAsync();
                }
            }

            return Ok("Created Successfully!");
        }
        catch (Exception ex)
        {
            //log error
            return StatusCode(500, ex.Message);
        }
    }
}