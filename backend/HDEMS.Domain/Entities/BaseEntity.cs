using FreeSql.DataAnnotations;
using HDEMS.Domain.Enums;

namespace HDEMS.Domain.Entities;

/// <summary>
/// 实体基类
/// </summary>
public abstract class BaseEntity
{
    [Column(IsPrimary = true, Position = 1)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 创建人
    /// </summary>
    [Column(Position = -10)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(ServerTime = DateTimeKind.Local, CanUpdate = false, Position = -9)]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    [Column(Position = -8)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column(ServerTime = DateTimeKind.Local, Position = -7)]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [Column(Position = -6)]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    [Column(Position = -5)]
    public string? DeletedBy { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [Column(Position = -4)]
    public DateTime? DeletedAt { get; set; }
}
