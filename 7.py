
"""
TODO
"""

import sys
import math

import direct.directbase.DirectStart
from pandac.PandaModules import Texture, TextureStage

base.setBackgroundColor(0.0, 0.0, 0.0)
base.disableMouse()

camera.setPos(0.0, -20.0, 10.0)
camera.lookAt(0.0, 0.0, 0.0)

root = render.attachNewNode("Root")

textureArrow = loader.loadTexture("arrow.png")
textureArrow.setWrapU(Texture.WMClamp)
textureArrow.setWrapV(Texture.WMClamp)

stageArrow = TextureStage("Arrow")
stageArrow.setSort(1)

textureCircle = loader.loadTexture("circle.png")

stageCircle = TextureStage("Circle")
stageCircle.setSort(2)

"""
TODO
exchange setSort
"""

modelCube = loader.loadModel("cube.egg")

cubes = []
for x in [-3.0, 0.0, 3.0]:
    cube = modelCube.copyTo(root)
    cube.setPos(x, 0.0, 0.0)
    cubes += [ cube ]

shader = loader.loadShader("7.sha")
root.setShader(shader)
root.setTexture(stageArrow, textureArrow)
root.setTexture(stageCircle, textureCircle)

base.accept("escape", sys.exit)
base.accept("o", base.oobe)

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
