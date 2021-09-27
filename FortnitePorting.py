bl_info = {
    "name": "Fortnite Porting",
    "author": "Half",
    "version": (0, 0, 1),
    "blender": (2, 90, 3),
    "location": "View3D > Sidebar > Fortnite Porting",
    "description": "Blender Addon for Fortnite Porting",
    "category": "Import-Export",
}

import json
import os
import re

import bpy
from bpy.props import StringProperty, BoolProperty, PointerProperty, EnumProperty, FloatProperty, FloatVectorProperty
from bpy.types import Operator, Panel, PropertyGroup, Scene, AddonPreferences

from io_import_scene_unreal_psa_psk_280 import pskimport, psaimport
from math import *
from mathutils import Matrix