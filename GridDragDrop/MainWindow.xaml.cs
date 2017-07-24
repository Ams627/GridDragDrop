using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        Point? _startPoint = null;
        public bool IsDragging { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MyList = new List<X>
            {
                new X { Name = "Fred", Number = 2 },
                new X { Name = "Jim", Number = 3 },
                new X { Name = "Sally", Number = 4 },
                new X { Name = "Sheila", Number = 5 }
            };
            this.DataContext = this;
            dg1.PreviewMouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
                {
                    var dg = s as DataGrid;
                    var selected = dg.SelectedItems;
                    foreach (var sel in selected)
                    {
                        Console.WriteLine($"selected: {((X)sel).Number}");
                    }

                    System.Diagnostics.Debug.WriteLine($"datagrid {(dg?.ToString() ?? "null")}");
                    Point mousePos = e.GetPosition(null);
                    System.Diagnostics.Debug.WriteLine($"POS: x={mousePos.X}, y={mousePos.Y}");

                    Vector diff = _startPoint.Value - mousePos;
                    // test for the minimum displacement to begin the drag
                    if (e.LeftButton == MouseButtonState.Pressed &&
                        (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                    {
                        var DataGridRow = FindAnchestor<DataGridRow>((DependencyObject)e.OriginalSource);

                        if (DataGridRow == null)
                            return;
                        // Find the data behind the DataGridRow
                        var dataTodrop = (X)dg.ItemContainerGenerator.
                            ItemFromContainer(DataGridRow);

                        if (dataTodrop == null) return;

                        // Initialize the drag & drop operation
                        var dataObj = new DataObject(dataTodrop);
                        dataObj.SetData("DragSource", s);
                        DragDrop.DoDragDrop(dg, dataObj, DragDropEffects.Copy);
                        _startPoint = null;
                        StartDrag(e);
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
                DataGridRow.IsSelected = !DataGridRow.IsSelected;
                

                System.Diagnostics.Debug.WriteLine($"down: x={_startPoint.Value.X}, y={_startPoint.Value.Y}");
            };

            void StartDrag(MouseEventArgs e)
            {
                System.Diagnostics.Debug.WriteLine("Drag");
            }

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
    }
}
