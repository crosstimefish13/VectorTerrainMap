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
构建拟合模型（或空间模型）是一种带有分析的构建过程，称为变差分析。在样本点的空间模型构建过程中，您首先需要创建一张经验变差图（empirical semivariogram），使用如下公式建立样本点**平面距离**（下文使用距离）与**测量值方差**（下文使用方差）的关系：

![公式12](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_09.gif)

其中：    
* ![公式13](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_17.gif) = 样本点 ![公式14](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_02.gif) 与 ![公式15](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_11.gif) 的距离
* ![公式16](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_08.gif) = 样本点 ![公式17](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_02.gif) 的测量值

该公式构建了样本点 ![公式18](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_02.gif) , ![公式19](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_11.gif) 的距离 ![公式20](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_17.gif) 与方差的关系 ![公式21](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_10.gif)。下图展示了某点（红色标记点）和它周围所有样本点的两两关系。需运用该公式计算完成所有样本点的两两关系。

![图片01](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_12.gif)    
*Figure 01 计算两点的方差*

通常，每两点有唯一的距离，然而样本点数量庞大。为了在可控范围内绘制所有两点关系结果，使用一个步长范围（lag bins）作为两点距离，而不是直接使用所有两点距离。比如，计算两点距离在 40 至 50 米间的所有方差的平均值，公式如下：

![公式22](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_14.gif)

其中：
* ![公式23](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_07.gif) = 需计算的两点对数量
* ![公式24](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_15.gif) = 各两点对的方差计算结果
* ![公式25](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_16.gif) = 方差平均值

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
## 球面函数模型
该模型描述了空间自相关性随两点距离递增而递减（即方差值增大），直到某两点距离之后的空间自相关性为零（方差值达到最大）。该模型是最常用的模型之一。

![图片03](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_18.gif)    
*Figure 03 球面函数模型*

## 指数函数模型
该模型被用来描述空间自相关性随两点距离递增呈指数级递减。在该模型中，自相关性仅在距离无限大处消失。指数模型也是一种常用模型。

![图片04](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_19.gif)    
*Figure 04 指数函数模型*

选择使用何种模型应基于数据的空间自相关性以及对空间场景的预先了解。
# 变差模型解析--Range, Sill 和 Nugget
如前所述，变差图描述了样本点的空间自相关性。由基本的地理学概念（越接近的事物越相似）可知，两样本点越接近则方差值越小。在计算并绘制完所有的两点关系后，选取一个变差模型进行拟合。通常使用值域（Range），阈值（Sill）和块值（Nugget）来描述这些变差模型。
## 值域和阈值
观察变差模型的函数曲线，您会注意到某些距离使得函数曲线趋于水平。使得函数曲线开始变平的距离值被称为值域。值域范围内的样本点距离越接近，具有空间自相关性，而值域外的点不再具有空间自相关性。

![图片05](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_20.gif)    
*Figure 05 值域，阈值和块值示例*

函数曲线中值域所对应的点（y 轴上的值）被称为阈值。偏阈值（Partial Sill）是阈值减去块值。有关块值的描述如下。
## 块值
理论上，距离为 0 （或者步长为 0）时函数曲线的方差值为 0。然而，在无限小的距离处，变差图通常展现出一种块效应，其方差值仍然大于 0。例如函数曲线与 y 轴的相交值为 2，则此时块值为 2。

块效应由测量误差或者空间数据源在小于采样间隔内的变异造成。测量误差由测量设备内的硬件误差造成。自然空间场景在尺度范围外仍具有空间变化，快效应部分来自于小于采样间隔的微小尺度上的变异。在采集数据前，获悉空间变异处的尺度是很重要的。
# 进行预测
当您已经估算出数据的空间自相关性即第一次使用数据（计算数据的空间信息并建模）后，您就可以使用拟合后的模型进行预测。此后将不再使用变差图。

您现在可以第二次使用数据进行预测。与 IDW 插值一样，Kriging 通过预测点周围样本点的测量值计算权重系数。在 IDW 插值中，样本点距离预测点越近，则对预测点的影响越大。然而，Kriging 中样本点权重系数的计算过程更复杂。IDW 使用基于距离的简单算法，但是 Kriging 中样本点的权重系数来自于经过数据处理后所得到的变差模型。为了构建连续的空间场景平面区域，预测点构成将构成一个平面区域网格，该网格基于变差模型且样本点分布在该网格中。

# Kriging 计算
通常有两种 Kriging 计算方法：标准法和通用法。

标准法是被广泛使用的最常用的 Kriging 计算方法，它是默认方法。标准法假设物理常数未知，在具有科学的原因前这是一种合理的假设。

通用法假设某种事物对样本数据具有绝对影响（比如全球季风），并且该影响能够在数学上被描述，例如被描述为一个多项式。原始样本点首先应去掉该多项式，即排除空间自相关性中的干扰。一旦构建完成变差模型，则需要将该多项式添加到预测点上以得到有意义的结果。仅当您的数据具有影响因素且该因素能够被数学描述时才应该使用通用法。

# Kriging 理论
Kriging 是一个复杂的过程，您需要具有空间统计学的知识才能理解本文。在使用 Kriging 前，您需要彻底了解 Kriging 的基本原理并且评估您的数据是否适用于 Kriging。如果您还未掌握该过程，强烈推荐您查阅本文末尾的参考文献。

Kriging 基于区域化变量理论，该理论中空间变化由空间场景中的 Z 值所描述，该理论假设空间变化在平面区域内具有统计均匀性（例如平面区域内所有可观测点的变化模式是相同的）。该统计均匀性假设是区域变化理论的基础。

# 数学模型
下面列出了几个数学模型的函数曲线和方程组，这些模型用来进行进行变差图的拟合。
