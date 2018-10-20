/* **********************************************************************************
 * Developed by Johan Fernandes based on the requirements and documents of 60-422 course
 * **********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using BookStoreLIB;

namespace BookStoreGUI {
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window {
        private static MainWindow AppWindow;
        DataSet dsBookCat;
        UserData userData;
        BookOrder bookOrder;

        private void loginButton_Click(object sender, RoutedEventArgs e) {
            var userData = new UserData();
            var dlg = new LoginDialog();
            dlg.Owner = this;
            dlg.ShowDialog();
            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true) {
                if (userData.LogIn(dlg.uname.Text, dlg.password.Password) == true) { 
                    this.statusTextBlock.Text = "You are logged in as User #" + userData.UserId;
                    AppWindow.Title = "Testing";        //Change Title
                }
                else
                    MessageBox.Show("You could not be verified. Please try again.");
            }
        }


        private void exitButton_Click(object sender, RoutedEventArgs e) { this.Close(); }
        public MainWindow() {
            InitializeComponent();
            AppWindow = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            BookCatalog bookCat = new BookCatalog();
            dsBookCat = bookCat.GetBookInfo();
            this.DataContext = dsBookCat.Tables["Category"];
            bookOrder = new BookOrder();
            userData = new UserData();
            this.orderListView.ItemsSource = bookOrder.OrderItemList;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            OrderItemDialog orderItemDialog = new OrderItemDialog();
            DataRowView selectedRow;
            selectedRow = (DataRowView)this.ProductsDataGrid.SelectedItems[0];
            orderItemDialog.isbnTextBox.Text = selectedRow.Row.ItemArray[0].ToString();
            orderItemDialog.titleTextBox.Text = selectedRow.Row.ItemArray[2].ToString();
            orderItemDialog.priceTextBox.Text = selectedRow.Row.ItemArray[4].ToString();
            orderItemDialog.Owner = this;
            orderItemDialog.ShowDialog();
            if (orderItemDialog.DialogResult == true)
            {
                string isbn = orderItemDialog.isbnTextBox.Text;
                string title = orderItemDialog.titleTextBox.Text;
                double unitPrice = double.Parse(orderItemDialog.priceTextBox.Text);
                int quantity = int.Parse(orderItemDialog.quantityTextBox.Text);
                bookOrder.AddItem(new OrderItem(isbn, title, unitPrice, quantity));
            }
        }
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.orderListView.SelectedItem != null)
            {
                var selectedOrderItem = this.orderListView.SelectedItem as OrderItem;
                bookOrder.RemoveItem(selectedOrderItem.BookID);
            }
        }
        private void chechoutButton_Click(object sender, RoutedEventArgs e)
        {
            int orderId;
            orderId = bookOrder.PlaceOrder(userData.UserId);
            MessageBox.Show("Your order has been placed. Your order id is " +
            orderId.ToString());
        }

    }
}
