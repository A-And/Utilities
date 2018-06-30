__FXPath=
__OutputPath=

for i in "$@"
do
    case $i in
        --fxpath=*)
                __FXPath=${i#*=}
                if [ ! -d $__FXPath ]; then
                    echo "FXPath $__FXPath does not exist"
                    exit 1  
                fi
        ;; 

        --outputPath=*)
                __OutputPath=${i#*=}
                if [ ! -d $outputPath ]; then
                    mkdir $outputPath
                fi
        ;;

        *)
            echo "Invalid arguments $i"
        ;;

    esac
done

if [ -z $__FXPath ]; then 
    echo "--fxpath is required"
    exit 1
fi

if [ -z $__OutputPath ]; then 
    echo "--outputpath is required"
    exit 1
fi


zip_tests() 
{
for testDir in $__FXPath/*/ ;
do
    for buildConfigDir in $testDir/*/ ; 
    do
        echo zip -r $__OutputPath/$(basename $buildConfigDir)/$(basename $testDir).zip $buildConfigDir
        pushd $buildConfigDir
        zip -r $__OutputPath/$(basename $buildConfigDir)/$(basename $testDir).zip ./*
        popd

    done
done
}

delete_results() {

for testDir in $__FXPath/*/ ;
do
    for buildConfigDir in $testDir/*/ ; 
    do
        for file in $buildConfigDir/* ;
        do
            if [ $(basename $file) == "testResults.xml" ]; then
                rm $file
            fi
        done

    done
done

}

zip_tests
