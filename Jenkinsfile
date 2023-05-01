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
            }			
			
        }
		
		stage('Deploy') {
			steps {   
				echo 'Deploying ...'

				sh 'docker compose down'
				
				sh 'rm -rf CollectionKeepersAPIV1/appsettings.*'
				sh 'cp ~/cred2/* CollectionKeepersAPIV1'

				sh 'docker build db -t database'

				sh 'docker build . -t ckbackend'				

				sh 'docker compose up -d'


				}
			}
		}
 }
