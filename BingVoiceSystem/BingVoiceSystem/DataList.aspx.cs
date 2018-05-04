using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class DataList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load data only on the initial page request
                // On postback, load the data after deleting rows
                ShowData();
            }
        }

        /// <summary>
        /// Binds the GridViews with data from the database.
        /// </summary>
        protected void ShowData()
        {
            DataGridView.DataSource = GlobalState.movieData.PrintData();
            DataGridView.DataBind();
        }

        /// <summary>
        /// Redirect to Add Data screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddDataButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DataListAdd.aspx");
        }

        /// <summary>
        /// Event to enter row editing mode for specific rule in the Data GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // NewEditIndex property is used to determine the index of the row being edited.
            DataGridView.EditIndex = e.NewEditIndex;
            ShowData();
        }

        /// <summary>
        /// Event to update the rows (data) edited in the Data GridView to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string oldMovie = DataGridView.DataKeys[e.RowIndex].Value.ToString();
            TextBox QuestionTextBox = (TextBox)DataGridView.Rows[e.RowIndex].Cells[0].Controls[0];
            TextBox AnswerTextBox = (TextBox)DataGridView.Rows[e.RowIndex].Cells[1].Controls[0];

            GlobalState.movieData.EditData(oldMovie, QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.GetUserName());
            DataGridView.EditIndex = -1;
            ShowData();
        }

        /// <summary>
        /// Event to cancel row editing mode in the Data GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
            DataGridView.EditIndex = -1;
            ShowData();
        }

        /// <summary>
        /// Event to delete a row (data) in the Data GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string movie = DataGridView.DataKeys[e.RowIndex].Value.ToString();
            GlobalState.movieData.DeleteData(movie);

            DataGridView.EditIndex = -1;
            ShowData();
        }
    }
}