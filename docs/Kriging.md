# Kriging 如何工作
Kriging 是一个高级地理统计过程，它通过一个离散点集合估算一个平面区域（surface），该点集合中的每个点应带有测量值（z-value）。与其他插值工具集提供的插值算法不同，在您选择最好的估值算法去建立平面区域前，您需要知道使用 Kriging 算法将涉及到对一个特定空间场景（phenomenon）的交互式研究，该空间场景由测量值所描述。
# Kriging 是什么
我们认为 IDW（inverse distance weighted，反向距离权重）算法和 Spline 算法是确定性的插值算法，因为它们直接基于空间场景内的测量值，或者基于严格的数学公式，这些方式明确了平面区域结果的光滑程度。另一类插值算法由地理统计方法构成，比如 Kriging，Kriging 基于统计模型，该模型带有自相关性（autocorrelation），该自相关性由样本点中的统计关系所体现。基于该模型，我们不仅可以通过地理统计技术预测平面区域，并且可以精准预测该平面区域内的一些测量值。

Kriging 假设样本点间的距离和方向反应了场景的空间相关性，该相关性描述了平面区域的变化方式。Kriging 算法尝试用一种数学方法计算每一个预测点的测量值，该方法需明确所有或某半径范围内的样本点数量。Kriging 由多个处理步骤组成：它包括对样本数据进行试探性（exploratory）统计分析；建立变差(variogram)模型；构造平面区域；并且（可选地）探测平面区域的变化。如果您认为样本点的距离和方向反应了空间相关性，那么 Kriging 是最合适的选择。Kriging 在土壤学和地理学上被广泛使用。
# Kriging方程
Kriging 与 IDW 类似，通过得到预测点周围各样本点的权重系数，来计算预测点的测量值。这两种插值算法的通用方程为样本点的权重系数求和：    
![公式01](https://github.com/crosstimefish13/VectorTerrainMap/blob/develop/docs/content/kriging/img/kriging_img_01.gif)    
其中：
