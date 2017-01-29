module.exports = function (grunt) {

    var path = require('path');

	// Load the package JSON file
	var pkg = grunt.file.readJSON('package.json');

	// get the root path of the project
	var projectRoot = 'src/' + pkg.name + '/';

	// Load information about the assembly
	var assembly = grunt.file.readJSON(projectRoot + 'Properties/AssemblyInfo.json');

	// Get the version of the package
	var version = assembly.informationalVersion ? assembly.informationalVersion : assembly.version;

	var os = require("os");
	var machineName = os.hostname();

	grunt.initConfig({
		pkg: pkg,
		clean: {
			files: [
				'releases/temp'
			]
		},
		copy: {
			binary: {
				files: [
					{
						expand: true,
						cwd: projectRoot + 'bin/',
						src: [
							pkg.name + '.dll',
							pkg.name + '.xml',
							'Skybrud.Essentials.dll',
							'Skybrud.Essentials.xml',
							'Skybrud.WebApi.Json.dll',
							'Skybrud.WebApi.Json.xml'
						],
						dest: 'releases/temp/bin/'
					}
				]
			},
			resources: {
			    files: [
					{
					    expand: true,
					    cwd: projectRoot + 'App_Plugins/' + pkg.name + '/',
					    src: ['**/*.*'],
					    dest: 'releases/temp/App_Plugins/' + pkg.name + '/'
					}
				]
			},
			nuget: {
			    files: [
					{
					    expand: true,
					    cwd: 'releases/nuget/',
					    src: [pkg.name + '.' + version + '.nupkg'],
					    dest: 'D:/NuGet/'
					}
				]
			}
		},
		nugetpack: {
			dist: {
				src: 'src/' + pkg.name + '/' + pkg.name + '.csproj',
				dest: 'releases/nuget/'
			}
		},
		zip: {
			release: {
			    router: function (filepath) {
			        var name = path.basename(filepath);
			        return name == 'LICENSE.html' ? name : filepath.replace('releases/temp/', '');
			    },
				src: [
					'releases/temp/**/*.*',
					projectRoot + '/LICENSE.html'
				],
				dest: 'releases/github/' + pkg.name + '.v' + version + '.zip'
			}
		},
		umbracoPackage: {
		    dist: {
		        src: 'releases/temp/',
		        dest: 'releases/umbraco',
		        options: {
		            name: pkg.name,
		            version: version,
		            url: pkg.url,
		            license: pkg.license.name,
		            licenseUrl: pkg.license.url,
		            author: pkg.author.name,
		            authorUrl: pkg.author.url,
		            readme: pkg.readme,
		            outputName: pkg.name + '.v' + version + '.zip'
		        }
		    }
		}
	});

	grunt.loadNpmTasks('grunt-contrib-clean');
	grunt.loadNpmTasks('grunt-contrib-copy');
	grunt.loadNpmTasks('grunt-nuget');
	grunt.loadNpmTasks('grunt-zip');
	grunt.loadNpmTasks('grunt-umbraco-package');

	grunt.registerTask('abjerner', ['clean', 'copy:binary', 'copy:resources', 'nugetpack', 'zip'/*, 'umbracoPackage'*/, 'clean', 'copy:nuget']);
	grunt.registerTask('release', ['clean', 'copy:binary', 'copy:resources', 'nugetpack', 'zip'/*, 'umbracoPackage'*/, 'clean']);

	grunt.registerTask('default', ['release']);

};