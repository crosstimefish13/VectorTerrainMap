# Kriging 如何工作
Kriging 是一个高级地理统计过程，它通过一个离散点集合估算一个平面区域（surface），该点集合中的每个点应带有测量值（z-value）。与其他插值工具集提供的插值算法不同，在您选择最好的估值算法去建立平面区域前，您需要知道使用 Kriging 算法将涉及到对一个特定空间场景（phenomenon）的交互式研究，该空间场景由测量值所描述。
# Kriging 是什么
我们认为 IDW（inverse distance weighted，反向距离权重）算法和 Spline 算法是确定性的插值算法，因为它们直接基于空间场景内的测量值，或者基于严格的数学公式，这些方式明确了平面区域结果的光滑程度。另一类插值算法由地理统计方法构成，比如 Kriging，Kriging 基于统计模型，该模型带有自相关性（autocorrelation），该自相关性由样本点中的统计关系所体现。基于该模型，我们不仅可以通过地理统计技术预测平面区域，并且可以精准预测该平面区域内的一些测量值。

Kriging 假设样本点间的距离和方向反应了场景的空间相关性，该相关性描述了平面区域的变化方式。Kriging 算法尝试用一种数学方法计算每一个预测点的测量值，该方法需明确所有或某半径范围内的样本点数量。Kriging 由多个处理步骤组成：它包括对样本数据进行试探性（exploratory）统计分析；建立变差（variogram）模型；构造平面区域；并且（可选地）探测平面区域的变化。如果您认为样本点的距离和方向反应了空间相关性，那么 Kriging 是最合适的选择。Kriging 在土壤学和地理学上被广泛使用。
# Kriging 方程
Kriging 与 IDW 类似，通过得到预测点周围各样本点的权重系数，来计算预测点的测量值。这两种插值算法的通用方程为样本点的权重系数求和：

![公式01](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_01.gif)

其中：    
* ![公式02](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_08.gif) = 第 ![公式03](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_03.gif) 个样本点 ![公式04](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_02.gif) 的测量值
* ![公式05](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_04.gif) = 第 ![公式06](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_03.gif) 个样本点 ![公式07](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_02.gif) 的权重系数
* ![公式08](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_06.gif) = 预测点 ![公式09](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_05.gif) 的测量估算值
* ![公式10](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_07.gif) = 样本点的数量

![公式11](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_04.gif) 在 IDW 中，仅依赖于样本点到预测点的距离。然而在 Kriging 中，权重不但基于上述距离，还基于所有样本点的空间分布。为了使用该空间分布计算权重，必须对空间自相关性进行量化。因此，在标准 Kriging 中需选取一个适当模型计算权重，该模型依赖于样本点和预测点的距离，以及预测点周围样本点的空间关系。下述章节将讨论如何运用 Kriging 通用方程创建平面区域并进行精确预测。
# 使用 Kriging 创建平面区域
Kriging 插值算法进行预测需要做两件事情：
* 找到某类依赖规则
* 进行预测

为了完成这两件事，Kriging 进行如下两步处理：
* 建立变差函数估算空间依赖值（自相关性的量化），该函数依赖于空间自相关性模型（一种拟合模型）
* 计算预测点的测量值（进行预测）

由上述两步可知 Kriging 使用了样本数据两次：第一次用来估算数据的空间自相关性；第二次进行预测计算。
# 变差分析（Variography）
构建拟合模型（或空间模型）是一种带有分析的构建过程，称为变差分析。在样本点的空间模型构建过程中，您首先需要创建一张经验变差图（empirical semivariogram），使用如下公式建立样本点平面距离与测量值方差的关系：

![公式12](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_09.gif)

其中：    
* ![公式13](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_17.gif) = 样本点 ![公式14](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_02.gif) 与 ![公式15](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_11.gif) 的平面距离
* ![公式16](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_08.gif) = 样本点 ![公式17](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_02.gif) 的测量值

该公式构建了样本点 ![公式18](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_02.gif) , ![公式19](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_11.gif) 的平面距离 ![公式20](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_17.gif) 与测量值方差的关系 ![公式21](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_10.gif)。下图展示了某点（红色标记点）和它周围所有样本点的两两关系。需运用该公式计算完成所有样本点的两两关系。

![图片01](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_12.gif)    
*Figure 01 计算两点对测量值的方差*

通常，每两点有唯一的平面距离，然而样本点数量庞大。为了在可控范围内绘制所有两点关系结果，使用一个步长范围（lag bins）作为两点平面距离，而不是使用所有两点平面距离。比如，计算两点平面距离在 40 至 50 米间的所有测量值方差的平均值，公式如下：

![公式22](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_14.gif)

其中：
* ![公式23](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_07.gif) = 需计算的两点对数量
* ![公式24](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_15.gif) = 各两点对的测量值方差计算结果
* ![公式25](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_16.gif) = 测量值方差平均值

经验变差图是一个坐标轴，y 轴为方差平均值，x 轴为两点对距离（或者步长），如下图所示。

![图片02](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_13.gif)    
*Figure 02 经验变差图示例*

空间自相关性明确了一个基本的地理学概念：两个事物越接近则越相似。因此，两点距离越近（变差图中越靠近 x 轴左端）则应有越相似的测量值（变差图中越靠近 y 轴下端）。两点距离越远（变差图中越靠近 x 轴右端）则它们越不相似因而有更大的方差值（变差图中越靠近 y 轴上端）。
# 使用经验变差图建立拟合模型
接下来需要对经验变差图中的离散点进行拟合。变差图建模是从空间描述到空间预测间的一个关键步骤。Kriging 的主要应用就是预测未测量点的测量值。经验变差图提供了数据的空间自相关性信息，然而它不能提供所有可能的方向和距离。因此，为了保证 Kriging 的预测值是正变化的（应始终位于坐标轴第一象限内），必须为经验变差图建立拟合模型。拟合模型为一段连续的曲线，通过函数进行描述。总的来说，它类似于回归分析，通过使用一段连续的直线或曲线对坐标点进行拟合。
# 变差模型
Kriging 算法提供了下列函数用以为经验变差图建模：
* 圆函数（Circular）
* 球面函数（Spherical）
* 指数函数（Exponential）
* 高斯函数（Gaussian）
* 线性函数（Linear）

选择不同模型将影响对未测量点的测量值预测，尤其是各函数曲线在接近坐标轴原点时的图形显著不同。曲线越接近原点越陡峭，则对预测结果的影响越大。就结果而言，构建的平面区域将不那么光滑。每个模型在不同场景下的拟合精确度各不相同。下文展示了两个常用函数模型以及两者的区别。
