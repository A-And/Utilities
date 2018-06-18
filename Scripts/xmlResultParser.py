import argparse
import os
import xml.etree.ElementTree as ET

def parseLog(testLog, failure_assemblies):
    logTree = ET.parse(testLog)
    if not logTree:
        print(testLog)
        
    root = logTree.getroot()
    assemblies = root.findall('assembly')
    if not assemblies:
        print(testLog)
    for assembly in assemblies:
        if assembly.get('failed') != '0' and assembly.get('failed') is not None:
            for collection in assembly.findall('collection'):
                if collection.get('failed') != '0' and collection.get('failed') is not None:
                    for test in collection.findall('test'):
                        if test.get('result') == 'Fail' is not None and test.get('result') == 'Fail' :
                            failure = test.find('failure')
                            if failure.get('exception-type') is not None and failure.get('exception-type') == 'System.IO.FileNotFoundException':
                                failure_msg = failure.find('message').text
                                if failure_msg is None:
                                    continue
                                start = failure.find('message').text.find('System.IO.FileNotFoundException : Could not load file or assembly ') 
                                end = failure.find('message').text.find(', Version=')
                                failure_assembly = failure_msg[start:end]
                                failure_assemblies.add(failure_assembly)

def main():
    parser = argparse.ArgumentParser(description='Process input strings')
    parser.add_argument('logPath', action='store', type=str)
    results = parser.parse_args()
    failure_assemblies = set()
    for subdirs, dirs, files in os.walk(results.logPath):
        for subdir in dirs:
            parseLog(os.path.join(results.logPath,subdir,"testResults.xml"), failure_assemblies)
    for assemblies in failure_assemblies:

        print(assemblies)
    
if __name__ == "__main__":
    main()