# PageGrid 使用说明

`PageGrid` 是 `Scm.View` 提供的分页数据网格，把「搜索栏 + 表格 + 分页 + 导出 + 抽屉编辑」组合成一个可复用组件。

## 1. 组成

| 类型 | 位置 | 职责 |
| --- | --- | --- |
| `PageGrid` | [PageGrid.xaml](../Scm.View/Controls/PageGrid.xaml) | 视图控件，数据上下文必须是 `ScmPageGridDvo` |
| `ScmPageGridDvo` | [ScmPageGridDvo.cs](../Scm.View/ScmPageGridDvo.cs) | 分页视图模型基类，统一分页导航 |
| `ScmPageGridDvo<T>` | 同上 | 泛型版本，提供强类型 `Items` |
| `ScmPageResult<T>` | 同上 | 单页数据 + 总行数 |
| `ScmColumnInfo` | [ScmColumnInfo.cs](../Scm.View/Models/ScmColumnInfo.cs) | 列元数据，同时驱动显示与导出 |
| `DataGridColumnsBehavior` | [DataGridColumnsBehavior.cs](../Scm.View/Controls/Attach/DataGridColumnsBehavior.cs) | 根据列元数据生成 DataGrid 列 |
| `GridExporter` | [GridExporter.cs](../Scm.View/Helper/GridExporter.cs) | 数据导出服务 |

PageGrid 只与 `DataContext` 上的 `ScmPageGridDvo` 交互，不依赖具体页面；页面通过依赖属性和事件接入。

## 2. 快速开始

### 2.1 定义明细类型

```csharp
public class UserItemDvo : ScmSearchResultItemDvo
{
    private string _codec;
    public string codec { get { return _codec; } set { SetProperty(ref _codec, value); } }

    private string _namec;
    public string namec { get { return _namec; } set { SetProperty(ref _namec, value); } }
}
```

明细类型必须继承 `ScmSearchResultItemDvo`（提供 `Id`、`IsChecked`、`IsEnabled` 等基础字段）。

### 2.2 定义 ViewModel

```csharp
public class UserViewDvo : ScmPageGridDvo<UserItemDvo>
{
    private ScmWindow _Window;

    public UserViewDvo(ScmWindow window)
    {
        _Window = window;

        // 列配置只需维护一次，显示和导出共用
        Columns = new List<ScmColumnInfo>
        {
            new ScmColumnInfo { Type = ScmColumnType.Text, Label = "ID", Value = "Id", Hidden = true },
            new ScmColumnInfo { Type = ScmColumnType.CheckBox, Label = "", Value = "IsChecked", Width = "60" },
            new ScmColumnInfo { Type = ScmColumnType.Text, Label = "编码", Value = "codec", Width = "160" },
            new ScmColumnInfo { Type = ScmColumnType.Text, Label = "名称", Value = "namec", Width = "*", MinWidth = "120" }
        };
    }

    protected override async Task<ScmPageResult<UserItemDvo>> LoadPageAsync()
    {
        var total = new RefAsync<int>();
        var list = await SqlHelper.GetSqlClient()
            .Queryable<UserDao>()
            .WhereIF(!string.IsNullOrEmpty(Key), a => a.codec.Contains(Key))
            .Select<UserItemDvo>()
            .ToPageListAsync(PageIndex, PageItems, total);

        return new ScmPageResult<UserItemDvo> { Items = list, Total = total.Value };
    }

    protected override void OnError(Exception ex)
    {
        _Window?.ShowException(ex);
    }
}
```

### 2.3 页面 XAML

```xml
<UserControl x:Class="Com.Scm.Views.UserView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:Uc="clr-namespace:Com.Scm.Controls;assembly=Scm.View">
    <Uc:PageGrid x:Name="PgGrid">
        <Uc:PageGrid.RowContextMenu>
            <ContextMenu>
                <MenuItem Header="查看" Click="MenuInfo_Click"/>
                <MenuItem Header="删除" Click="MenuDelete_Click"/>
            </ContextMenu>
        </Uc:PageGrid.RowContextMenu>
    </Uc:PageGrid>
</UserControl>
```

### 2.4 页面代码

```csharp
public partial class UserView : UserControl, ScmView
{
    private UserViewDvo _Dvo;

    public void Init(ScmWindow owner)
    {
        _Dvo = new UserViewDvo(owner);

        PgGrid.DataContext = _Dvo;   // 必须设置 DataContext
        _Dvo.FirstPageAsync();       // 首次加载需手动触发
    }

    public UserControl GetView()
    {
        return this;
    }
}
```

> `DataContext` 必须是 `ScmPageGridDvo` 的派生实例，否则 `Items`、`Columns`、分页等绑定全部失效。

## 3. 布局区域与可见性

PageGrid 分为四个区域，均由 ViewModel 上的 `Visibility` 属性控制：

| 区域 | 内容 | 控制属性 | 默认值 |
| --- | --- | --- | --- |
| 表头 | 左侧自定义区 + 右侧搜索框、查询按钮、高级按钮 | `HeadVisibility` | `Visible` |
| 表格 | DataGrid（行号、全选列、列由 `Columns` 生成） | — | 始终显示 |
| 表尾 | 左侧操作按钮、右侧分页器 | `FootVisibility` | `Visible` |
| 搜索区 | 搜索框与查询按钮 | `SearchVisibility` | `Visible` |
| 分页区 | 分页器 | `PageVisibility` | `Visible` |
| 操作区 | 刷新 / 打印 / 导出 / 选项 | `OptionVisibility` | `Collapsed` |

示例：只读列表页隐藏操作区与分页：

```csharp
public UserViewDvo(ScmWindow window)
{
    FootVisibility = Visibility.Collapsed;
    // 或
    OptionVisibility = Visibility.Collapsed;
    PageVisibility = Visibility.Collapsed;
}
```

设为 `Visible` 时对应区域才出现，设为 `Collapsed` 即隐藏。

## 4. 数据与分页

### 4.1 属性

| 属性 | 默认值 | 说明 |
| --- | --- | --- |
| `PageItems` | `20` | 每页行数 |
| `PageIndex` | `1` | 当前页，从 1 开始 |
| `PageCount` | `0` | 总页数，加载后自动计算 |
| `ItemCount` | `0` | 总行数，加载后自动回填 |
| `Key` | `null` | 搜索关键字，绑定到搜索框 |
| `Items` | 空集合 | 当前页数据（只读集合，直接增删） |
| `Columns` | `null` | 列配置，`protected set`，只能在 ViewModel 内赋值 |

### 4.2 方法

```csharp
public Task SearchAsync(int pageIndex = 0);  // >0 跳转到指定页，0 刷新当前页
public Task ReloadAsync();                   // 刷新当前页
public Task FirstPageAsync();                // 首页
public Task PrevPageAsync();                 // 上一页
public Task NextPageAsync();                 // 下一页
public Task EndPageAsync();                  // 末页
protected abstract Task<ScmPageResult<T>> LoadPageAsync();
protected virtual void OnError(Exception ex); // 默认 Growl.Error(ex.Message)
```

- `PageCount` 由基类统一计算：`ceil(Total / PageItems)`，子类不需要维护。
- 「查询」「刷新」按钮内部调用 `SearchAsync(0)`；分页器翻页调用 `SearchAsync(页码)`。
- 单选/多选页内操作后，调用 `ReloadAsync()` 刷新当前页即可。

### 4.3 搜索条件

简单关键字用 `Key`；复杂条件在 ViewModel 上自行声明属性，再在 `LoadPageAsync` 中读取：

```csharp
private ScmRowStatusEnum _status;
public ScmRowStatusEnum Status { get { return _status; } set { SetProperty(ref _status, value); } }

protected override async Task<ScmPageResult<UserItemDvo>> LoadPageAsync()
{
    // Status / Key 均在此处作为查询条件
}
```

搜索面板放在 `SearchContent`（见第 9 节），点击抽屉里的「查询」会执行 `SearchAsync(0)`。

## 5. 列配置

### 5.1 ScmColumnInfo 字段

| 字段 | 类型 | 默认值 | 说明 |
| --- | --- | --- | --- |
| `Type` | `ScmColumnType` | `Text` | 列类型 |
| `Label` | `string` | — | 列标题，也是导出时的表头 |
| `Value` | `string` | — | 绑定路径（CLR 属性名，**区分大小写**） |
| `Width` | `string` | — | 列宽，见 5.3 |
| `MinWidth` | `string` | — | 最小宽度，仅数字生效 |
| `MaxWidth` | `string` | — | 预留字段，当前未生效 |
| `Align` | `ScmColumnAlign` | `None` | 文本对齐：`Left` / `Center` / `Right` / `Stretch` |
| `Format` | `ScmColumnFormat` | `None` | 模板列格式：`Icon` / `Number` / `DateTime` |
| `Hidden` | `bool` | `false` | 是否隐藏该列，可通过表尾「选项」面板切换 |
| `Export` | `bool` | `true` | 是否参与导出 |
| `ReadOnly` | `bool` | `true` | 文本列是否只读 |
| `Sortable` | `bool` | `false` | 是否允许用户排序 |
| `Mode` | `BindingMode` | `OneWay` | 绑定模式 |
| `Converter` | `IValueConverter` | `null` | 值转换器 |
| `Foreground` | `string` | `null` | 前景色绑定路径（仅 `Icon` 格式使用） |
| `Background` | `string` | `null` | 背景色绑定路径（仅 `Icon` 格式使用） |

### 5.2 列类型

| `ScmColumnType` | 生成的列 | 说明 |
| --- | --- | --- |
| `Text` | `DataGridTextColumn` | 最常用；`Align` 控制文本对齐 |
| `CheckBox` | `DataGridCheckBoxColumn` | 表头为全选复选框，`Value` 一般为 `IsChecked`，点击表头会批量设置明细的 `IsChecked` |
| `Status` | `DataGridTemplateColumn` + `ToggleButton` | 绑定布尔属性，使用 `ToggleButtonSwitch` 样式（开关） |
| `Template` | `DataGridTemplateColumn` | 需配合 `Format` 使用，见下表 |
| `ComboBox` | — | 暂未实现，该列会被忽略 |

`Format` 仅在 `Type = Template` 时生效：

| `ScmColumnFormat` | 渲染内容 |
| --- | --- |
| `Icon` | `PackIconMaterial` 图标；`Value` 需为 `PackIconKind`，可用 `Converter` 转换；可绑定 `Foreground` / `Background` |
| `Number` | 右对齐文本，格式 `N2` |
| `DateTime` | 文本，格式 `yyyy-MM-dd HH:mm:ss` |
| `None` | 空单元格 |

### 5.3 宽度写法

| 写法 | 效果 |
| --- | --- |
| `"*"` | 按比例填充剩余宽度 |
| `"120"` | 固定 120 像素 |
| 不填 | 默认 100 像素 |

### 5.4 完整示例

```csharp
Columns = new List<ScmColumnInfo>
{
    new ScmColumnInfo { Type = ScmColumnType.Text,     Label = "ID",       Value = "Id",        Hidden = true },
    new ScmColumnInfo { Type = ScmColumnType.CheckBox, Label = "",         Value = "IsChecked", Width = "60" },
    new ScmColumnInfo { Type = ScmColumnType.Text,     Label = "编码",     Value = "codec",     Width = "160", Sortable = true },
    new ScmColumnInfo { Type = ScmColumnType.Text,     Label = "名称",     Value = "namec",     Width = "*", MinWidth = "120" },
    new ScmColumnInfo { Type = ScmColumnType.Status,   Label = "启用",     Value = "IsEnabled", Width = "80" },
    new ScmColumnInfo { Type = ScmColumnType.Template, Label = "创建时间", Value = "CreateTime", Width = "160", Format = ScmColumnFormat.DateTime },
    new ScmColumnInfo { Type = ScmColumnType.Template, Label = "图标",     Value = "IconKind",  Width = "60",  Format = ScmColumnFormat.Icon },
};
```

### 5.5 运行时调整列的显示与顺序

表尾「选项」按钮会打开列设置面板（需 `OptionVisibility = Visible`）：

- 勾选 / 取消复选框：立即显示或隐藏该列，等价于修改 `ScmColumnInfo.Hidden`；
- 点击 ↑ / ↓：调整列的显示顺序，同时调整 `Columns` 集合的顺序；
- 已处理首项不可上移、末项不可下移。

调整结果只作用于当前 ViewModel 实例（不落盘），并遵循以下约定：

- 顺序同步到 `Columns`，因此下次导出时列顺序与界面一致；
- 隐藏列仍会参与导出，是否导出只由 `Export` 字段决定。

## 6. 交互事件

### 6.1 单元格双击：`CellDoubleClick`

冒泡路由事件，触发前已自动选中该行；双击表头或空白区域不会触发。

```xml
<Uc:PageGrid x:Name="PgGrid" CellDoubleClick="PgGrid_CellDoubleClick"/>
```

```csharp
private void PgGrid_CellDoubleClick(object sender, PageGridCellEventArgs e)
{
    var item = e.Item as UserItemDvo;
    if (item == null)
    {
        return;
    }

    // e.Cell 为当前单元格，双击行号时为 null
    // e.Row  为当前行
    LoadDetail(item);
}
```

| 成员 | 说明 |
| --- | --- |
| `e.Item` | 行数据 |
| `e.Row` | `DataGridRow` |
| `e.Cell` | `DataGridCell`；双击行号时为 `null` |

也可以在外层容器统一挂载：`AddHandler(PageGrid.CellDoubleClickEvent, handler)`。

### 6.2 右键菜单：`RowContextMenu`

菜单打开前，PageGrid 会先把该行设为选中项，再把菜单的 `DataContext` 设为行数据。因此在菜单项中：

- `{Binding 属性名}` 直接是当前行字段；
- 事件里 `((MenuItem)sender).DataContext` 就是当前行对象。

```xml
<Uc:PageGrid x:Name="PgGrid">
    <Uc:PageGrid.RowContextMenu>
        <ContextMenu>
            <MenuItem Header="查看详情" Click="MenuInfo_Click"/>
            <MenuItem Header="复制编码" Click="MenuCopy_Click"/>
            <Separator/>
            <MenuItem Header="删除" Click="MenuDelete_Click" IsEnabled="{Binding IsEnabled}"/>
        </ContextMenu>
    </Uc:PageGrid.RowContextMenu>
</Uc:PageGrid>
```

```csharp
private void MenuInfo_Click(object sender, RoutedEventArgs e)
{
    var menuItem = sender as MenuItem;
    var item = menuItem == null ? null : menuItem.DataContext as UserItemDvo;
    if (item == null)
    {
        return;
    }

    ShowInfo(item);
}
```

注意事项：

- 空白区域和表头右键不弹菜单；
- 批量操作请读取 `PgGrid.SelectedItems`，不要依赖右键所在的单行；
- 菜单标题可以绑定行字段，如 `Header="{Binding namec, StringFormat=删除 {0}}"`。

## 7. 抽屉面板

页面可以用 PageGrid 内置的右侧抽屉承载详情、编辑和高级查询。

```csharp
// 只读详情：不显示任何按钮
PgGrid.ShowInfo(detailView);

// 编辑：显示「确认」「取消」，「确认」回调返回 true 才关闭
PgGrid.ShowEdit(editView, Save);

// 高级查询：显示「查询」「取消」
PgGrid.ShowSearch(searchView);

private bool Save()
{
    if (!_Dvo.IsValid())
    {
        return false;
    }

    // 保存逻辑
    return true;
}
```

`SaveDelegate` 定义：`public delegate bool SaveDelegate();`。返回 `false` 时抽屉保持打开，便于用户修正。

> `ShowSearch` 中的「查询」按钮执行的是列表的 `SearchAsync(0)`，搜索条件由查询视图与 ViewModel 共享。

## 8. 选中项

| 成员 | 说明 |
| --- | --- |
| `SelectedItem` | 当前选中行数据，可读写 |
| `SelectedItems` | 多选行集合 |
| `SelectedIndex` | 选中行索引 |
| `SelectedValue` | 选中值 |
| `SelectedCells` | 选中单元格集合 |

## 9. 自定义区域

| 依赖属性 | 类型 | 说明 |
| --- | --- | --- |
| `CustomContent` | `UIElement` | 显示在表头左侧，常用于「新增」等按钮；为 `null` 时自动隐藏 |
| `SearchContent` | `FrameworkElement` | 高级查询面板；非空时表头出现「高级」按钮，点击后在抽屉中显示 |

```xml
<Uc:PageGrid x:Name="PgGrid">
    <Uc:PageGrid.CustomContent>
        <Button Content="新增" Style="{StaticResource ButtonPrimary}" Click="BtAdd_Click"/>
    </Uc:PageGrid.CustomContent>

    <Uc:PageGrid.SearchContent>
        <StackPanel Margin="12">
            <TextBlock Text="状态"/>
            <ComboBox SelectedValue="{Binding Status}" .../>
        </StackPanel>
    </Uc:PageGrid.SearchContent>
</Uc:PageGrid>
```

## 10. 数据导出

点击表尾「导出」会弹出保存对话框，支持 `xlsx` / `xls` / `csv` / `json` / `sql`。

规则：

- 仅导出 `Export = true` 的列；
- 列顺序与 `Columns` 顺序一致，Excel / CSV 表头使用 `Label`；
- 只导出当前页数据（`DataGrid.ItemsSource`）；
- JSON 输出对象数组，键为 `Label`；
- SQL 输出 `INSERT INTO table_name (...) VALUES (...);`，表名需自行替换，字符串中的单引号会自动转义。

也可以直接调用导出服务：

```csharp
await GridExporter.ExportXls(items, columns, "d:/data.xlsx");
await GridExporter.ExportCsv(items, columns, "d:/data.csv");
await GridExporter.ExportJson(items, columns, "d:/data.json");
await GridExporter.ExportSql(items, columns, "d:/data.sql");
await GridExporter.ExportAsync(items, columns); // 弹保存对话框
```

## 11. 内置按钮

| 按钮 | 行为 |
| --- | --- |
| 查询 | `SearchAsync(0)` |
| 刷新 | `SearchAsync(0)` |
| 导出 | 见第 10 节 |
| 选项 | 打开列设置面板：勾选控制显示、↑ / ↓ 调整顺序（见 5.5） |
| 打印 | 暂无逻辑 |
| 列表 / 单页 | 暂无逻辑 |
| 高级 | 仅设置 `SearchContent` 后显示 |

## 12. 完整示例

| 场景 | 文件 |
| --- | --- |
| 本地 SQLite 分页查询 | [MainView.xaml](../Scm.Samples/Views/Native/MainView.xaml)、[MainViewDvo.cs](../Scm.Samples/Views/Native/MainViewDvo.cs) |
| 远程 API 分页查询 | [SearchParamsDvo.cs](../Scm.Samples/Views/Remote/SearchParamsDvo.cs) |

## 13. 常见问题

1. **表格空白、无数据**
   - 确认 `PgGrid.DataContext` 是 `ScmPageGridDvo` 派生实例；
   - 确认已调用 `FirstPageAsync()` / `SearchAsync()`；
   - 不要在自己的 ViewModel 里再声明 `Items` 字段，会遮蔽基类集合导致页面绑定的始终是空集合。

2. **某一列没有值**
   - `Value` 必须与属性的 CLR 名称完全一致，绑定区分大小写（`codec` 不等于 `Codec`）。

3. **分页器一直是 0 页 / 无法翻页**
   - `LoadPageAsync` 必须返回正确的 `Total`；
   - `PageItems` 不能为 0。

4. **列没有出现**
   - 检查 `Type`；`ComboBox` 与 `Format = None` 的模板列不会生成内容。

5. **双击 / 右键没反应**
   - 双击表头或空白区域不会触发 `CellDoubleClick`；
   - 未设置 `RowContextMenu` 时不会弹出右键菜单；
   - 空白区域与表头右键被主动屏蔽。

6. **导出内容缺列**
   - 该列 `Export` 被设为 `false`，或列配置中没有该列。
