
"""
This is a basic example without any shader. If you do not understand a line,
please have a look at the Panda3D manual, there is not a single bit of magic in
this file.
"""

import sys

import direct.directbase.DirectStart
from direct.interval.LerpInterval import LerpFunc
from pandac.PandaModules import Texture, TextureStage

base.setBackgroundColor(0.0, 0.0, 0.0)
base.disableMouse()

base.camLens.setNearFar(1.0, 50.0)
base.camLens.setFov(45.0)

camera.setPos(0.0, -20.0, 10.0)
camera.lookAt(0.0, 0.0, 0.0)

root = render.attachNewNode("Root")
root.setPos(0.0, 0.0, 0.0)

textureArrow = loader.loadTexture("arrow.png")
textureArrow.setWrapU(Texture.WMClamp)
textureArrow.setWrapV(Texture.WMClamp)

stageArrow = TextureStage("Arrow")
stageArrow.setSort(1)

textureCircle = loader.loadTexture("circle.png")
textureCircle.setWrapU(Texture.WMClamp)
textureCircle.setWrapV(Texture.WMClamp)

stageCircle = TextureStage("Circle")
stageCircle.setSort(2)

"""
Please open the model with a text editor. Egg files are human readable. We need
this information later to understand how the vertex shader and the fragment
shader work. Some insights:

The cube has six faces. Each face has four different vertices. Therefore the cube has
24 vertices. Theoretically a cube only need eight vertices, here each vertex
has to be shared by three faces. Problem is, that here each vertex can only
have one color, but what happens if we want that each of the six faces has an
another color? This is impossible if the cube is only defined with eight vertices.
There are more disadvantages if we only define the cube with eight vertices, we talk
later about. The only advantage with less vertices is that we have to send less
vertices to the graphic card, but in almost all applications vertices are not a
limiting factor. The memory consumption of vertices in contrast to the memory
consumption of textures is negligible. As already written each vertex has one
associated color. Besides the color there is one UV coordinate for every vertex.

It may be useful if you modify the vertices manually and see the results.
"""

modelCube = loader.loadModel("cube.egg")

"""
One more thing about colors. When you look close at the colors inside the file
you may see that there are eight different colors defined. Black, White, Red,
Green, Blue, Yellow, Purple and Cyan. Each color is repeated three times. When
you run this sample you may ask yourself, why does this cube has thousands of
colors then? Who creates this nice gradients along the edges? We use a new word
here: Linear Interpolation. Today graphic cards are very good at linear
interpolation. They can do billions of linear interpolations per second. The
downside is that sometimes the graphic card can ONLY do linear interpolation and
you have no choice to change that, even with a shader. Back to the colors. If
you have a red color (1.0, 0.0, 0.0) on one vertex and a dark blue color (0.0,
0.0, 0.5) on the other vertex the graphic card simply interpolates the color for
every pixel between this two vertices, even without shaders (only if requested
but Panda3D ask the graphic card to do this). For this consideration the graphic
card does not know that a color consists of a R(ed), G(reen), B(lue) and maybe
A(lpha) part. The graphic card interpolates each part of a color for itself.
Here is an example what a graphic card does:

100% at red vertex, 0% at dark blue vertex => 1.0, 0.0, 0.0
75% at red vertex, 25% at dark blue vertex => 0.75, 0.0, 0.125
50% at red vertex, 50% at dark blue vertex => 0.5, 0.0, 0.25
25% at red vertex, 75% at dark blue vertex => 0.25, 0.0, 0.375
0% at red vertex, 100% at dark blue vertex => 0.0, 0.0, 0.5

A bit oversimplified (in reality it does not work exactly like this but the
result is the same): If the graphic card likes to draw a pixel on a screen it
first looks if this pixel is on a vertex. If yes it can directly take this color
and draw a pixel with this color. If it is not, the graphic card looks to which
triangle this pixel belongs. Then it looks were the vertices of this triangle
are and calculates the distance to each of this vertices. Based on this distance
and the color of this vertices it interpolates all color components and draws a
pixel with this color.

We already said that the graphic card does not care about the fact that a color
consists of the three parts R, G and B. The good thing about this is that the
graphic card can do the calculations for R independent of the other parts. This
is true for G an B. You may ask what should I care? The advantage is that the
graphic card can do this in parallel. A graphic card is in general extremely
specialized in parallel computing. This also true for vertex shaders and pixels
shaders. Each calculation for a vertex or pixel is done individually. A vertex
never knows how his neighbor looks like and a pixel never knows what his
neighbors color is. This is a reason why graphic card vendors can improve the
performance of GPUs faster then CPUs. Vertex and pixel shaders are inherently
parallel. The disadvantage of this is that if anyone likes to do some
calculations with respect to the neighborhood he has to create a complex setup
that often (but not always) is not fast enough for 60+ FPS games.

A blur (like in the glow example) filter is an example of such a setup. You need
at least two passes to create such an effect.
"""

cubes = []
for x in [-3.0, 0.0, 3.0]:
    cube = modelCube.copyTo(root)
    cube.setPos(x, 0.0, 0.0)
    cubes += [ cube ]

"""
DIRTY
Look at the textures first with any image viewer. Enable the textures afterwards
and see how they are blended together with the colors.
"""
#root.setTexture(stageArrow, textureArrow)
#root.setTexture(stageCircle, textureCircle)

"""
DIRTY
Also try to enable the textures not on all cubes at the same time. To test this
remove/comment the preceding two lines.
"""
#cubes[0].setTexture(stageArrow, textureArrow)
#cubes[1].setTexture(stageCircle, textureCircle)
#cubes[2].setTexture(stageArrow, textureArrow)
#cubes[2].setTexture(stageCircle, textureCircle)

"""
Exit the application.
"""

base.accept("escape", sys.exit)

"""
Use oobe mode to move around the camera.
"""

base.accept("o", base.oobe)

"""
Start an interval that rotate all cubes independent.
"""

def animate(t):
    for i in range(len(cubes)):
        cubes[i].setH(t * (i + 1))

interval = LerpFunc(animate, 5.0, 0.0, 360.0)

base.accept("i", interval.start)

"""
Move all cubes (you should understand why all three and not only one).
"""

def move(x, y, z):
    root.setX(root.getX() + x)
    root.setY(root.getY() + y)
    root.setZ(root.getZ() + z)

base.accept("d", move, [1.0, 0.0, 0.0])
base.accept("a", move, [-1.0, 0.0, 0.0])
base.accept("w", move, [0.0, 1.0, 0.0])
base.accept("s", move, [0.0, -1.0, 0.0])
base.accept("e", move, [0.0, 0.0, 1.0])
base.accept("q", move, [0.0, 0.0, -1.0])

run()
