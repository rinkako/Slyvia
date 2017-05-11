﻿## 并行调度机制
YRE支持场景多线程，即当场景的逻辑正在被执行时，它所包含的其中一些函数也可以同步地被执行。这个特性称之为**并行处理**，YRE中使用并行调度机制来支持该特性。<br/>

### 使用并行处理
YRE是这样区分一个函数是并行的还是非并行的：当一个场景下的函数的名字是以`synr_`开头（大小写敏感），那么它是一个并行处理函数，YRE会在进行场景加载和切换的时候，自动地启动该场景下所有的并行处理函数的消息循环。<br/>
需要注意的是，并行处理函数会忽略传递给它的参数列表。如果要传递参数，请使用**全局变量**。

### 并行调度策略和并行安全
并行处理函数的消息循环的刷新频率是固定的，它没有主消息循环的快启动机制，而是按照标准时钟事件的模式进行消息循环。每次时钟事件触发的间隔不早于一个`GlobalConfigContext.DirectorTimerInterval`字段所对应的值（单位：万分之一毫秒），默认情况下**1毫秒**刷新一次。<br/>
当一个场景下包含多个并行处理函数时，YRE会在将场景栈帧压完主调用堆栈后，以它们名字的字典序依次启动它们。但是，YRE不保证这些并行处理函数在全局上都是执行有序的，也不保证它们之间的动作操作是并行安全，因此在使用并行处理函数时，请务必仔细考虑当前采用的游戏动作是否是**并行安全**的。一个不安全的例子是：是并行处理函数中使用了动画特效来让一个图片精灵循环旋转，但却在主调用堆栈上使用了**等待动画结束**这个并行不安全指令，这将会导致主调用堆栈的消息循环一直被阻塞在等待动画结束的指令上。要解决这一问题，请在主调用堆栈上使用并行安全的**延时等待**指令。

### 并行处理函数的启动和结束
并行处理函数的启动是在进行场景切换的时候进行的。它按照这样的逻辑顺序来执行：

- 在切换场景，停下当前已有的并行处理函数，把新场景的栈帧压入主调用堆栈
- 调用`RuntimeManager`的`ConstructParallel`方法，它取出当前场景的并行处理器向量`ParallellerContainer`并为向量中的每一个`SceneFunction`创建一个并行处理执行器`ParallelExecutor`，该执行器包括一个带优先级时钟对象`DispatcherTimer`和一个YRE栈机`StackMachine`
- 将函数对象封装为函数栈帧并提交给栈机，启动时钟对象开始消息循环，并压并行状态栈（后述）。消息循环处理函数是`Director`中的`ParallelUpdateContext`方法，它与主消息循环方法不同，在执行完该函数栈帧的所有指令后不会弹栈，而是会将栈帧的`IP`指针移回开头，循环执行
- 当再次发生场景切换时，就要停止上一个并行状态。它扫描当前并行状态栈的栈顶，将该栈帧中存放的所有并行处理器的时钟对象关闭

### 并行状态栈
在某些特殊情况下，YRE可能会触发**场景调用**，例如，进行右键菜单时。这种情况下主调用堆栈可能同时存在两个场景栈帧，此时YRE会停下不处于栈顶的场景所属的那些并行处理器，并且需要在该场景栈帧重新成为栈顶时，并行处理器要恢复工作。这就需要引入一个并行状态记录栈。<br/>
并行记录栈的栈帧是**同一栈帧偏移量下的主调用堆栈上的场景所对应的并行处理器所组成的一个向量**。因此，在发生场景调用时，首先应该将并行状态栈栈顶向量中的所有并行执行器停止，然后启动新的并行处理器，并将新的并行处理器向量压入并行状态栈的栈顶；当主调用堆栈场景栈帧出栈时，并行状态栈就要停止当前栈顶的所有并行执行器，并弹出该栈帧，然后继续将新栈顶的栈帧中所有的并行处理器的时钟对象启动，恢复到上一个并行状态。