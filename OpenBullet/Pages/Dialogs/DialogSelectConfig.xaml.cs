using RuriLib;
using RuriLib.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per DialogSelectConfig.xaml
    /// </summary>
    public partial class DialogSelectConfig : Page
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        object Caller { get; set; }

        private string searchString = "";
        public string SearchString { get { return searchString; } set { searchString = value; } }

       

        public DialogSelectConfig(object caller, int op)
        {
            InitializeComponent();
            Caller = caller;
            if (op == 3)
            {
                DataContext = Globals.mainWindow.ConfigsPage.ConfigManagerPage.DataContext;
                
            }
            else if (op == 2)
            {
                Globals.mainWindow.ConfigsPage.rere();
                DataContext = Globals.mainWindow.ConfigsPage.ConfigManagerPageX.DataContext;
            }
            else if (op == 1)
            {
                Globals.mainWindow.ConfigsPage.rere();
                DataContext = Globals.mainWindow.ConfigsPage.ConfigManagerPageS.DataContext;
            }

           

        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            //filterTextbox.Text;
            
        }

        

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if (configsListView.SelectedItems.Count == 0) return;
            if(Caller.GetType() == typeof(Runner))
            {
                if (Globals.obSettings.General.LiveConfigUpdates) ((Runner)Caller).SetConfig(((ConfigViewModel)configsListView.SelectedItem).Config);
                else ((Runner)Caller).SetConfig(IOManager.CloneConfig(((ConfigViewModel)configsListView.SelectedItem).Config));
            }
            ((MainDialog)Parent).Close();
        }

        private void SelectOnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (configsListView.SelectedItems.Count == 0) return;
            if (Caller.GetType() == typeof(Runner))
            {
                if (Globals.obSettings.General.LiveConfigUpdates) ((Runner)Caller).SetConfig(((ConfigViewModel)configsListView.SelectedItem).Config);
                else ((Runner)Caller).SetConfig(IOManager.CloneConfig(((ConfigViewModel)configsListView.SelectedItem).Config));
            }
            ((MainDialog)Parent).Close();
        }


        private void listViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                configsListView.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            configsListView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void filterTextbox_KeyDown(object sender, TextChangedEventArgs e)
        {
           
            //    configsListView.Items.IndexOf(item);
           // CollectionViewSource.GetDefaultView(configsListView.ItemsSource).Refresh();
        }
    }
}
