
"""
TODO
"""

import sys
import math

from direct.interval.LerpInterval import LerpFunc
import direct.directbase.DirectStart

base.setBackgroundColor(0.0, 0.0, 0.0)
base.disableMouse()

base.camLens.setNearFar(1.0, 50.0)
base.camLens.setFov(45.0)

camera.setPos(0.0, -20.0, 10.0)
camera.lookAt(0.0, 0.0, 0.0)

root = render.attachNewNode("Root")

modelCube = loader.loadModel("cube.egg")

cubes = []
for x in [-3.0, 0.0, 3.0]:
    cube = modelCube.copyTo(root)
    cube.setPos(x, 0.0, 0.0)
    cubes += [ cube ]

shader = loader.loadShader("5.sha")
root.setShader(shader)
root.setShaderInput("panda3drocks", 1.0, 0.0, 1.0, 1.0)

base.accept("escape", sys.exit)
base.accept("o", base.oobe)

def animate(t):
    c = abs(math.cos(math.radians(t)))
    root.setShaderInput("panda3drocks", c, c, c, 1.0)

    #r = abs(math.cos(math.radians(t + 0.0)))
    #g = abs(math.cos(math.radians(t + 10.0)))
    #b = abs(math.cos(math.radians(t + 20.0)))
    #cubes[0].setShaderInput("panda3drocks", r, 0.0, 0.0, 1.0)
    #cubes[1].setShaderInput("panda3drocks", 0.0, g, 0.0, 1.0)
    #cubes[2].setShaderInput("panda3drocks", 0.0, 0.0, b, 1.0)

interval = LerpFunc(animate, 5.0, 0.0, 360.0)

base.accept("i", interval.start)

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
