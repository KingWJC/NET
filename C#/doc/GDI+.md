# 绘制虚线

画笔用来填充图形内部,

钢笔则用来绘制带有一定宽度,样式和色彩的线条和曲线.

## DashStyle枚举 

指定用 Pen 对象绘制的虚线的样式

1. Custom 指定用户定义的自定义划线段样式。
2. Dash 指定由划线段组成的直线。
3. DashDot 指定由重复的划线点图案构成的直线。
4. DashDotDot 指定由重复的划线点点图案构成的直线。
5. Dot 指定由点构成的直线。
6. Solid 指定实线。

## 线帽样式

1. StartCap 获取或设置用在通过此 Pen 对象绘制的直线起点的帽样式
2. EndCap 获取或设置用在通过此 Pen 对象绘制的直线终点的帽样式
3. DashCap 获取或设置用在短划线终点的帽样式，这些短划线构成通过此 Pen 对象绘制的虚线

**LineCap 枚举：**

成员名称  说明    

AnchorMask  指定用于检查线帽是否为锚头帽的掩码。 

ArrowAnchor  指定箭头状锚头帽。    

Custom  指定自定义线帽。   

DiamondAnchor  指定菱形锚头帽。   

Flat  指定平线帽。   

NoAnchor  指定没有锚。   

Round  指定圆线帽。    

RoundAnchor  指定圆锚头帽。    

Square  指定方线帽。   

SquareAnchor  指定方锚头帽。   

Triangle  指定三角线帽。 

## 线条接头样式

LineJoin 枚举：成员名称 说明

1. Bevel 指定成斜角的联接。这将产生一个斜角。
2. Miter 指定斜联接。这将产生一个锐角或切除角，具体取决于斜联接的长度是否超过斜联接限制。
3. MiterClipped 指定斜联接。这将产生一个锐角或斜角，具体取决于斜联接的长度是否超过斜联接限制。
4. Round 指定圆形联接。这将在两条线之间产生平滑的圆弧

## 示例

```
Pen pen = new Pen(Brushes.Black, 1);
pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
pen.StartCap = System.Drawing.Drawing2D.LineCap.NoAnchor;
pen.EndCap = System.Drawing.Drawing2D.LineCap.NoAnchor;
pen.LineJoin = System.Drawing.Drawing2D.LineJoin.MiterClipped;
pen.DashPattern = new float[] { 5, 5 };
e.Graphics.DrawLine(pen, new Point(1, 5), new Point(pcDrugCompute.Width, 5));
```

