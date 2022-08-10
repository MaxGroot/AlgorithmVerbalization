# Think Aloud Decision Tree Learning
This was a thesis project of 2018-2019 and is no longer actively maintained.

## Basic functionality
The program accepts several inputs. From an INI file, it reads the settings and parameters for operation. It allows both model validation and training. 

When in training, the program imports the data using the DataController, transforming a CSV file to an internal representation of instances of the DataInstance class, which are collected in a ObservationSet. 

The program also imports a set of InferenceTypes using the VocabularyController and bundles them into a Vocabulary instance. It does so from reading the contents of a JSON file. A InferenceType might require a State Descriptor, which is also constructed from the description provided in the vocabulary file. Vocabularies for both C4.5 and ID3 have been provided as an example. 
(These vocabularies were used in an experiment setting and their results and feasibility have as such been discussed with further recommendations indicated but not implemented in this solution)

Having imported both the vocabulary and the training data, the program constructs an instance to keep track of inferences made: an instance of InferenceManager. It also constructs an instance of SnapShot, that can produce intermediary snapshots of decision trees to be able to output prediction models at several moments during training.

Finally, the program constructs an agent. This agent is provided with the algorithm that will be used, as well as the inference manager that the agent require access to. When the user opts to start training, the TRAIN method is called, providing both dataset and parameters. The agent then calls the TRAIN method on the provided algorithm. The agent passes itself to the algorithm, so the algorithm can call the agent to construct inferences during runtime. 

Within an algorithm’s implementation code, the algorithm can call AGENT.Think(string inference_id). It can provide a set of keys and values to provide the variables and their values to be considered. The agent constructs an instance of StateRecording, provided with the proper State Descriptor. The variables are set according to how the algorithm has provided them, and the algorithm can also call to finish the inference. When an inference is finished, it verifies if all required variables have been set and if they have the correct type. After training, the inferences are saved. If the program attempts to save an unfinished inference, this means that there is a disparity between the provided vocabulary and the implementation of said vocabulary in the code, and an error is thrown. 

At moments of choice, but at least after training, the prediction model is passed to the snapshot instance. This classifies the training data using that model, to establish its accuracy and to colour the leaves within the decision tree based on the amount of instances they classify as a means of visual representation. The prediction model is then saved in a format of decision rules, a format than can be imported for classification, and a graphical representation in the DOT language that can be opened with a program such as GVEdit.
