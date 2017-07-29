using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using MvvmFoundation.Wpf;

namespace GridDragDrop
{
    public class X
    {
        public string Name { get; set; }
        public int Number { get; set; }
    }
    public partial class MainWindow : Window
    {
        public List<X> MyList { get; set; }
        public List<X> MyList2 { get; set; }
        bool deselecting = true;
        Point? _startPoint = null;
        public bool IsDragging { get; set; }

        public RelayCommand DropCommand { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DropCommand = new RelayCommand(() =>
            {
                System.Diagnostics.Debug.WriteLine("Dropped");
            });
            MyList = new List<X>
            {
                new X { Name = "Fred", Number = 2 },
                new X { Name = "Jim", Number = 3 },
                new X { Name = "Sally", Number = 4 },
                new X { Name = "Sheila", Number = 5 }
            };
            MyList2 = new List<X>();

            this.DataContext = this;
            dg1.PreviewMouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
                {
                    var dg = s as DataGrid;
                    var selected = dg.SelectedItems;

                    Point mousePos = e.GetPosition(null);

                    Vector diff = _startPoint.Value - mousePos;
                    // test for the minimum displacement to begin the drag
                    if (e.LeftButton == MouseButtonState.Pressed &&
                        (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                    {
                        string dataToDrop = null;
                        var grid = FindAnchestor<DataGrid>((DependencyObject)e.OriginalSource);

                        if (grid != null)
                        {
                            var rows = grid.SelectedItems.Cast<X>();
                            if (rows != null)
                            {
                                foreach (var row in rows)
                                {
                                    dataToDrop += row.Name + " " + row.Number.ToString() + "\n";
                                }
                            }
                        }

                        if (dataToDrop != null)
                        {
                            // Initialize the drag & drop operation
                            var dataObj = new DataObject(DataFormats.Text, dataToDrop);
                            dataObj.SetData("DragSource", s);
                            DragDrop.DoDragDrop(dg, dataObj, DragDropEffects.Copy);
                        }

                        _startPoint = null;
                    }
                }
            };

            dg1.PreviewMouseLeftButtonDown += (s, e) =>
            {
                _startPoint = e.GetPosition(null);
                e.Handled = true;

                var dg = s as DataGrid;
                dg.Focus();
                var DataGridRow = FindAnchestor<DataGridRow>((DependencyObject)e.OriginalSource);
                if (!DataGridRow?.IsSelected == true)
                {
                    DataGridRow.IsSelected = true;
                    deselecting = false;
                }
                else
                {
                    deselecting = true;
                }
            };

            dg1.PreviewMouseLeftButtonUp += (s, e) =>
            {
                _startPoint = e.GetPosition(null);
                e.Handled = true;

                var dg = s as DataGrid;
                dg.Focus();
                var DataGridRow = FindAnchestor<DataGridRow>((DependencyObject)e.OriginalSource);
                if (deselecting && DataGridRow != null)
                {
                    DataGridRow.IsSelected = false;
                }
            };
        }

        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void dg2_Drop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Dropped");
        }
    }
}
