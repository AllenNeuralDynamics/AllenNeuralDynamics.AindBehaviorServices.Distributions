# /// script
# dependencies = [
#   "aind_behavior_services==0.13.3",
# ]
# ///

from aind_behavior_services.task.distributions import Distribution
from aind_behavior_services.schema import export_schema
from pydantic import BaseModel
import os


class _Container(BaseModel):
    distribution: Distribution


with open(
    os.path.join(
        "./src/AllenNeuralDynamics.AindBehaviorServices.Distributions/distributions.json"
    ),
    "w",
    encoding="utf-8",
) as f:
    json_model = export_schema(_Container)
    f.write(json_model)
