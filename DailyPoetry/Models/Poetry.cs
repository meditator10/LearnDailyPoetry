using SQLite;

namespace DailyPoetry.Models;

[Table("works")]
public class Poetry
{
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Column("author_name")]
    public string Author { get; set; } = string.Empty;
    [Column("dynasty")] 
    public string Dynasty { get; set; } = string.Empty;
    [Column("content")]
    public string Content { get; set; } = string.Empty;


    // 虚拟属性：Snippet
    // 功能：显示诗词的预览字段,通过现有属性Content的第一个句号切开的方式获取
    // 因为数据库表中没有Snippet属性，所以用Ignore忽略关联
    private string _snippet;
    [Ignore]
    public string Snippet => _snippet ??= Content.Split('。')[0];

}
