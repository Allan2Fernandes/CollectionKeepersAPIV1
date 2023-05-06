pipeline {
    agent any

    stages {
        stage('Start up') {
			steps {
				echo 'Starting up'
			}
		}
		
		stage('Build') {
            steps {
                echo 'Building..'
				sh 'docker build . -t ckbackend'
            }
        }
		
		stage('Test') {
            steps {
                echo 'Testing..'	
				sh 'dotnet test'
				dir('XUnitTestProject'){
					//Unit tests
					sh 'rm coverage.cobertura.xml'
					sh 'dotnet add package coverlet.collector'
					sh 'dotnet add package coverlet.msbuild'
					sh "dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:ExcludeByFile='**/*Migrations/*.cs'"
					//Load tests
					sh 'k6 run --vus 15 --duration 10s LoadTests/CustomTest.js'
					//sh 'k6 run LoadTests/SoakTest.js'
					//sh 'k6 run LoadTests/LoadTest.js'
					//sh 'k6 run LoadTests/SpikeTest.js'
					//sh 'k6 run LoadTests/StressTest.js'
				}
            }		
			post {
				success {
					archiveArtifacts 'XUnitTestProject/coverage.cobertura.xml'
					publishCoverage adapters: [istanbulCoberturaAdapter(path: 'XUnitTestProject/coverage.cobertura.xml', thresholds: [
						[failUnhealthy: true, thresholdTarget: 'Conditional', unhealthyThreshold: 80.0, unstableThreshold: 50.0]
					])], checksName: '', sourceFileResolver: sourceFiles('NEVER_STORE')
				}
			}		
        }
		
		
		stage('Deploy') {
			steps {   
				echo 'Deploying ...'

				sh 'docker compose down'
				
				sh 'rm -rf CollectionKeepersAPIV1/appsettings.*'
				sh 'cp ~/cred2/* CollectionKeepersAPIV1'

				sh 'chmod 775 -R db/'
				sh 'docker build db -t database'
				sh 'docker build . -t ckbackend'
				sh 'docker compose up -d'


				}
			}
		}
 }
